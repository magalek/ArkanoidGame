using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct BlockInfo {
    public static BlockInfo Empty => new BlockInfo(null, -1);
    
    public BlockBase Block;
    public int ID;

    public BlockInfo(BlockBase block, int id) {
        Block = block;
        ID = id;
    }
}

[Serializable]
public class BlocksData {
    public Vector2Int gridSize;
    public List<BlockInfo> blocks;

    public BlocksData(Vector2Int gridSize, List<BlockInfo> blocks) {
        this.gridSize = gridSize;
        this.blocks = blocks;
    }
}

public class BlockManager : MonoManager {

    private const int SEQUENCE_GENERATOR_ITERATION_LIMIT = 20;

    public event Action<List<BlockInfo>> GeneratedBlocks;
    public event Action<BlockBase> BlockDestroyed;

    [SerializeField] private List<BlockBase> blocks = new List<BlockBase>();
    [SerializeField] private Vector2 blockSize;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float halfChangeChance;

    [SerializeField] private Transform blocksParent;

    private List<BlockInfo> spawnedBlocks = new List<BlockInfo>();
    private int blockCount;

    private int[] blockSequence;
    private int currentHorizontalOffset = 3;
    private int currentSequenceLength;

    private Vector3 startMarkerPosition;

    List<int> gridXEvenDividers;

    private GameManager gameManager;

    protected override void Awake() {
        base.Awake();
        startMarkerPosition = transform.GetChild(0).position;

        CalculateDividers(gridSize.x);
    }

    private void OnEnable() {
        gameManager = ManagerService.Instance.Get<GameManager>();
    }

    private void Start() {
        gameManager.Restarting += OnRestarting;
        gameManager.StartingNew += OnRestarting;
        gameManager.Continuing += OnContinuing;
    }

    private void OnRestarting() {
        GenerateBlocks();
    }

    private void OnContinuing(SaveState state) {
        GenerateLoadedBlocks(state.BlocksData);
    }

    private void CalculateDividers(int target) {
        gridXEvenDividers = new List<int>(target);
        for (int i = 2; i < target; i++) {
            if (i % 2 == 0 && target % i == 0) gridXEvenDividers.Add(i);
        }
    }

    public void GenerateBlocks() {
        ClearBlocks();

        CalculateSequence();

        bool shouldReturn = UnityEngine.Random.value > 0.5f;
        bool returning = false;
        int returningY = 0;

        for (int y = 0; y < gridSize.y; y++) {
            if ((y + 1) == gridSize.y / 2 && UnityEngine.Random.value > 1f - halfChangeChance) {
                ChangeShape();
            }

            if (shouldReturn) {
                if (returningY + 1 > currentHorizontalOffset) returning = true;
                if (returningY - 1 < 0) returning = false;
                returningY += returning ? -1 : 1;
            }
            for (int x = 0; x < gridSize.x; x++) {
                int sequenceIndex = (x % currentSequenceLength);
                int offsetDiff = shouldReturn ? returningY : (y % currentHorizontalOffset);
                int blockIndex = (sequenceIndex + offsetDiff) % blockSequence.Length;
                Vector2 position = startMarkerPosition + new Vector3(blockSize.x * x, blockSize.y * y);
                var blockToSpawnIndex = blockSequence[blockIndex];
                SpawnBlock(position, blockToSpawnIndex);
            }
        }

        GeneratedBlocks?.Invoke(spawnedBlocks);
    }

    private void SpawnBlock(Vector2 position, int blockToSpawnIndex) {
        if (blockToSpawnIndex > -1) {
            var block = Instantiate(blocks[blockToSpawnIndex], position, Quaternion.identity, blocksParent);
            block.Initialize(RaiseBlockDestroyedEvent);
            spawnedBlocks.Add(new BlockInfo(block, blockToSpawnIndex));
            blockCount++;
        }
        else {
            spawnedBlocks.Add(BlockInfo.Empty);
        }
    }

    public BlocksData GetBlocksData() {
        List<BlockInfo> infos = new List<BlockInfo>();
        foreach (var blockInfo in spawnedBlocks) {
            if (blockInfo.Block == null) infos.Add(BlockInfo.Empty);
            else infos.Add(blockInfo);
        }

        return new BlocksData(gridSize, infos);
    }

    public void GenerateLoadedBlocks(BlocksData data) {
        if (data == null) return;
        ClearBlocks();
        blockCount = 0;
        int index = 0;
        for (int y = 0; y < data.gridSize.y; y++) {
            for (int x = 0; x < gridSize.x; x++) {
                Vector2 position = startMarkerPosition + new Vector3(blockSize.x * x, blockSize.y * y);
                var blockToSpawnIndex = data.blocks[index].ID;
                SpawnBlock(position, blockToSpawnIndex);
                index++;
            }
        }
        GeneratedBlocks?.Invoke(spawnedBlocks);
    }

    private void RaiseBlockDestroyedEvent(BlockBase block) {
        BlockDestroyed?.Invoke(block);
        blockCount--;
        if (blockCount == 0) gameManager.StartNextLevel();
    }

    private void ClearBlocks() {
        foreach (var blockInfo in spawnedBlocks) {
            if (blockInfo.Block) Destroy(blockInfo.Block.gameObject);
        }
        spawnedBlocks.Clear();
    }

    private void ChangeShape() {
        CalculateDividers(gridSize.x);
        CalculateSequence();
    }

    private void CalculateSequence() {
        currentSequenceLength = gridXEvenDividers[UnityEngine.Random.Range(0, gridXEvenDividers.Count)];

        int overflowCounter = 0;
        int currentIndex = UnityEngine.Random.Range(-1, blocks.Count);
        do {
            blockSequence = new int[currentSequenceLength];
            for (int i = 0; i < currentSequenceLength; i++) {
                blockSequence[i] = (currentIndex + UnityEngine.Random.Range(-1, blocks.Count)) % blocks.Count;
            }
            overflowCounter++;
        } while (!SequenceVaries() && overflowCounter <= SEQUENCE_GENERATOR_ITERATION_LIMIT);

        if (overflowCounter > SEQUENCE_GENERATOR_ITERATION_LIMIT) Debug.LogWarning("Sequence generator limit reached.");

        currentHorizontalOffset = UnityEngine.Random.Range(1, currentSequenceLength);
    }

    private bool SequenceVaries() {
        for (int i = 1; i < blockSequence.Length; i++) {
            if (blockSequence[i - 1] == -1 || blockSequence[i] == -1) continue;
            if (blockSequence[i - 1] != blockSequence[i]) return true;
        }
        return false;
    }

    private void OnDestroy() {
        gameManager.Restarting -= OnRestarting;
        gameManager.StartingNew -= OnRestarting;
        gameManager.Continuing -= OnContinuing;
    }
}
