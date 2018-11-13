namespace SimpleCPUMiner.Model
{
    public class Algo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsCpuSupport { get; set; }
        public bool IsGpuSupport { get; set; }

        public Algo(){}
    }
}
