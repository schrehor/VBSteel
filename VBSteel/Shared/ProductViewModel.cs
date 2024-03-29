namespace VBSteel.Shared;

public class ProductViewModel
{
	public string Name { get; set; }
	public byte[] ImageData { get; set; }
	public string ImageType { get; set; }
	public Guid ProductId { get; set; }
	public string Description { get; set; }
}