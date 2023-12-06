using System.ComponentModel.DataAnnotations.Schema;

namespace VBSteel.Server;

[Table("Forms")]
public class FormData
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Text { get; set; }
}