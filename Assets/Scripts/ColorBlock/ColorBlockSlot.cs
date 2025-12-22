using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.ColorBlock
{
    public class ColorBlockSlot : MonoBehaviour
    {
        private ColorBlock_Block block;
        public bool IsEmpty()
        {
            return block == null;
        }

        public void ResetSlot()
        {
            block = null;
        }

        public void SetBlock(ColorBlock_Block block)
        {
            this.block = block;
        }

        public ColorBlock_Block GetBlock()
        {
            return block;
        }
    }
}