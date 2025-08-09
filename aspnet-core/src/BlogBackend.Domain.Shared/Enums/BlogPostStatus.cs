namespace BlogBackend.Enums
{
    /// <summary>
    /// 博客文章状态
    /// </summary>
    public enum BlogPostStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        Draft = 0,
        
        /// <summary>
        /// 已发布
        /// </summary>
        Published = 1,
        
        /// <summary>
        /// 已归档
        /// </summary>
        Archived = 2,
        
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted = 3
    }
}
