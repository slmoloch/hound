using BusinessEntityLab.Entities;
using BusinessEntityMappers;

namespace BusinessEntityLab.Mappers
{
	class AttachmentSaveMapper : BusinessEntitySaveMapper<Attachment>
	{
		public AttachmentSaveMapper(ISaver<Attachment> saver) : base(saver)
		{
		}
	}
}
