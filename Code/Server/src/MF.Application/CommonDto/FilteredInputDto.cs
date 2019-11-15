namespace MF.CommonDto
{
    /// <summary>
    /// ��ҳ ���� ���� ����
    /// </summary>
    public class FilteredInputDto :IFilteredResultRequest
    {
        /// <summary>
        /// ��������
        /// </summary>
        public string Filter { get; set; }
    }
}
