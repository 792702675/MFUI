using Aliyun.OSS;

namespace MF.OSS
{
    public class GetPartsInput
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public int? MaxParts { get; set; }
        public int? PartNumberMarker { get; set; }
        public string EncodingType { get; set; }
        public string UploadId { get; set; }

        public ListPartsRequest ToRequest()
        {
            return new ListPartsRequest(BucketName,Key,UploadId)
            {
                MaxParts = MaxParts,
                PartNumberMarker = PartNumberMarker ,
                EncodingType = EncodingType
            };
        }
    }
}