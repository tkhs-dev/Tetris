namespace TetrisCore.Source
{
    public class Cell
    {
        private Block _block;
        public Block Block => _block;

        public Cell()
        {
        }

        public void SetBlock(Block block)
        {
            _block = block;
        }

        public void RemoveBlock()
        {
            _block = null;
        }

        public bool HasBlock()
        {
            return _block != null;
        }
    }
}