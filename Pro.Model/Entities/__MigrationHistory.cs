using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pro.Model
{
    /// <summary>
    /// -
    /// </summary>
    [Table("__MigrationHistory")]
    public partial class __MigrationHistory
    {

		[Key, Column(Order = 0)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[StringLength(150)]
		public string MigrationId { get; set; }

		[Key, Column(Order = 1)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[StringLength(300)]
		public string ContextKey { get; set; }

		public byte[] Model { get; set; }

		[Required]
		[StringLength(32)]
		public string ProductVersion { get; set; }

    }
}
