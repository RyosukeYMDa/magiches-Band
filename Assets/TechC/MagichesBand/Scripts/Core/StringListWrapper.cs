using System.Collections.Generic;

namespace TechC.MagichesBand.Core
{
    /// <summary>
    /// Listをシリアライズ可能な形で包むためのラッパークラス
    /// Unityのシリアライズ機能で保存や読み書きを可能にするために使う
    /// </summary>
    [System.Serializable]
    public class StringListWrapper
    {
        public List<string> idList;

        /// <summary>
        /// コンストラクタでリストを受け取り初期化する
        /// 引数として渡されたリストをこのクラスのidListに代入する
        /// </summary>
        /// <param name="idList">初期化用の文字列リスト</param>
        public StringListWrapper(List<string> idList)
        {
            // 引数のリストをそのまま保持するために代入する
            this.idList = idList; 
        }
    }
}