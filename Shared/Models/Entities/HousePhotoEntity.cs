using System;

namespace Shared.Models.Entities
{
	public class HousePhotoEntity
	{
		public Guid PhotoId { get; set; }

		public Guid HouseId { get; set; }

		public string FileName { get; set; } = string.Empty;

		// MIME type (image/jpeg, image/png, etc.)
		public string ContentType { get; set; } = string.Empty;

		/*
         * Azure Blob Storage design:
         * Container: house-photos
         * Blob name example:
         * houses/{houseId}/photos/{photoId}.jpg
         */
		public string BlobStorageKey { get; set; } = string.Empty;

		public DateTime UploadedAtUtc { get; set; }
	}
}
