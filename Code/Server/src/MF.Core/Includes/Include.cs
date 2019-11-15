using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MF.Includes
{
    /// <summary>
    /// 弱约束，使用的时候自己看着办
    /// </summary>
    public static class Include
    {
        public static IQueryable<TEntity> IncludeQuestionAllObject<TEntity>(this IQueryable<TEntity> source, string prefixNavigationPropertyPath = null) where TEntity : class
        {
            if (!prefixNavigationPropertyPath.IsNullOrEmpty())
            {
                prefixNavigationPropertyPath = prefixNavigationPropertyPath.EnsureEndsWith('.');
            }
            return source
                .Include(prefixNavigationPropertyPath + "QuestionSkills.Skill")
                .Include(prefixNavigationPropertyPath + "Analysis")
                .Include(prefixNavigationPropertyPath + "Note")
                .Include(prefixNavigationPropertyPath + "AnswerArea")
                .Include(prefixNavigationPropertyPath + "Answers.Text")
                .Include(prefixNavigationPropertyPath + "Answer");
        }
        public static IQueryable<TEntity> IncludeItemAllObject<TEntity>(this IQueryable<TEntity> source, string prefixNavigationPropertyPath = null) where TEntity : class
        {
            if (!prefixNavigationPropertyPath.IsNullOrEmpty())
            {
                prefixNavigationPropertyPath = prefixNavigationPropertyPath.EnsureEndsWith('.');
            }
            return source.Include(prefixNavigationPropertyPath + "Collection.Animation.Frames");
        }
        public static IQueryable<TEntity> IncludeDressGroupAllObject<TEntity>(this IQueryable<TEntity> source, string prefixNavigationPropertyPath = null) where TEntity : class
        {
            if (!prefixNavigationPropertyPath.IsNullOrEmpty())
            {
                prefixNavigationPropertyPath = prefixNavigationPropertyPath.EnsureEndsWith('.');
            }
            return source
                .Include(prefixNavigationPropertyPath + "HumanBodys.HumanBodyModelings.Collection.Animation.Frames")
                .Include(prefixNavigationPropertyPath + "DressMainAdornments.Collection.Animation.Frames")
                .Include(prefixNavigationPropertyPath + "DressSecondAdornments.Collection.Animation.Frames")
                .Include(prefixNavigationPropertyPath + "DressDialogs.Collection.Animation.Frames")
                .Include(prefixNavigationPropertyPath + "DressBackgrounds.Collection.Animation.Frames");
        }

    }
}
