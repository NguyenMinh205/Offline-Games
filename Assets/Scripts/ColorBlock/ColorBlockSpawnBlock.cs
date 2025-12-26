using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.ColorBlock
{
    public class ColorBlockSpawnBlock : MonoBehaviour
    {
        [SerializeField] private List<ColorBlock_Block> blockPrefabs;
        [SerializeField] private List<ColorBlockSlot> slots;

        public List<ColorBlockSlot> Slots => slots;

        private Dictionary<ColorBlockSlot, Dictionary<ColorBlock_Block, Queue<ColorBlock_Block>>> slotPools = new Dictionary<ColorBlockSlot, Dictionary<ColorBlock_Block, Queue<ColorBlock_Block>>>();

        public void Init()
        {
            if (slotPools.Count > 0) return;

            foreach (ColorBlockSlot slot in slots)
            {
                slotPools[slot] = new Dictionary<ColorBlock_Block, Queue<ColorBlock_Block>>();

                foreach (ColorBlock_Block prefab in blockPrefabs)
                {
                    slotPools[slot][prefab] = new Queue<ColorBlock_Block>();
                }
            }
        }

        private ColorBlock_Block GetBlock(ColorBlockSlot slot, ColorBlock_Block prefab)
        {
            Queue<ColorBlock_Block> pool = slotPools[slot][prefab];
            ColorBlock_Block block;

            if (pool.Count > 0)
            {
                block = pool.Dequeue();
                block.gameObject.SetActive(true);
                block.transform.SetParent(slot.transform);
            }
            else
            {
                block = Instantiate(prefab, slot.transform);
                block.OriginalPrefab = prefab;
            }

            return block;
        }

        public void ReturnBlock(ColorBlockSlot slot, ColorBlock_Block block)
        {
            block.ResetTilePositions();
            block.gameObject.SetActive(false);
            slotPools[slot][block.OriginalPrefab].Enqueue(block);
        }

        public void Spawn()
        {
            foreach (ColorBlockSlot slot in slots)
            {
                slot.ResetSlot();
            }

            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsEmpty())
                {
                    int index = Random.Range(0, blockPrefabs.Count);
                    ColorBlock_Block newBlock = GetBlock(slots[i], blockPrefabs[index]);
                    newBlock.transform.localPosition = Vector3.zero;
                    newBlock.transform.localScale = Vector3.one;
                    newBlock.InitBlock(slots[i]);
                    slots[i].SetBlock(newBlock);
                }
            }
        }

        public void CheckAndSpawn()
        {
            bool allEmpty = true;
            foreach (ColorBlockSlot slot in slots)
            {
                if (!slot.IsEmpty())
                {
                    allEmpty = false;
                    break;
                }
            }

            if (allEmpty)
            {
                Spawn();
            }
        }

        public void ReturnAllBlocksToPool()
        {
            foreach (ColorBlockSlot slot in slots)
            {
                if (!slot.IsEmpty())
                {
                    ColorBlock_Block block = slot.GetBlock();
                    if (block != null)
                    {
                        ReturnBlock(slot, block);
                        slot.ResetSlot();
                    }
                }
            }
        }
    }
}