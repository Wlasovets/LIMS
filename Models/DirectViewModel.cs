namespace Web.Models
{
    public class DirectViewModel
    {
        public int SampleId { get; set; }
        public bool DirectToSelect { get; set; }
        public bool DirectToToxicology { get; set; }
        public bool DirectToBacteriology { get; set; }
        public bool DirectToChemicalLab { get; set; }
        public bool DirectToRadiology { get; set; }
    }
}