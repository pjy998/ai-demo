namespace BlogBackend.Enums
{
    /// <summary>
    /// 博客评论状态
    /// </summary>
    public enum BlogCommentStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// 已审核通过
        /// </summary>
        Approved = 1,
        
        /// <summary>
        /// 已拒绝
        /// </summary>
        Rejected = 2,
        
        /// <summary>
        /// 垃圾评论
        /// </summary>
        Spam = 3
    }
}
