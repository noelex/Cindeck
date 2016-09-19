using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck.Core
{
    /// <summary>
    /// スキル効果の継続時間。
    /// </summary>
    public enum SkillDuration
    {
        /// <summary>
        /// 効果は一瞬の間だけ継続する
        /// </summary>
        Instantaneous,

        /// <summary>
        /// 効果はわずかな間継続する
        /// </summary>
        Short,

        /// <summary>
        /// 効果は少しの間継続する
        /// </summary>
        Medium,

        /// <summary>
        /// 効果はしばらくの間継続する
        /// </summary>
        Long,

        /// <summary>
        /// 効果はかなりの間継続する
        /// </summary>
        VeryLong
    }
}
