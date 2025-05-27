using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        public List<BlockData> blockDatas;

        public List<Brick> blocks;
        public Transform blocksParent;

        public static GameManager Instance;

        public int[][] board;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            board = new int[10][]
            {
                new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, 0, 0, 0, 0, 0, 0, 0, 0, -1 },
                new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
            };
            SpawnBlocks(board);
        }

        public void SpawnBlocks(int[][] board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    var blockData = blockDatas.Find(data => data.id == board[i][j]);
                    if (blockData != null)
                    {
                        var block = Instantiate(
                            blockData.prefab,
                            new Vector3(i * 5, 0, j * 5),
                            Quaternion.identity
                        );
                        block.transform.SetParent(blocksParent);
                        blocks.Add(block);
                    }
                }
            }
            blocksParent.position = new Vector3(
                -board.Length * 5 / 2,
                0,
                -board[0].Length * 5 / 2
            );
        }
    }

    [System.Serializable]
    public class BlockData
    {
        public int id;
        public Brick prefab;
    }
}
