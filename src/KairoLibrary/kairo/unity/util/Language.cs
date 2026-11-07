using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using java.lang;
using java.net;
using java.util;
using kairo.common.cfg;
using kairo.unity.analytics;
using kairo.unity.io;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.util
{
	public class Language
	{
		private class LangPackFormat
		{
			public string File;

			public char LineCode = '\n';

			public char SegCode = '\t';

			public LangPackFormat(string file, char lineCode, char segCode)
			{
				File = file;
				LineCode = lineCode;
				SegCode = segCode;
			}
		}

		private struct Position
		{
			public int Line;

			public int Segment;

			public Position(int line, int segment)
			{
				Line = line;
				Segment = segment;
			}
		}

		private struct RecordPosition
		{
			public int Line;

			public int Segment;

			public LangPackRecord Record;

			public RecordPosition(int line, int segment, LangPackRecord record = null)
			{
				Line = line;
				Segment = segment;
				Record = record;
			}
		}

		private class LangPackRecord
		{
			public int Id;

			public string File;

			public List<Position> Positions = new List<Position>();

			public List<string> From = new List<string>();

			public List<string> To = new List<string>();

			public LangPackRecord(int id, string file, string text)
			{
				Id = id;
				File = file;
				From.Add(text);
			}

			public void AddPosition(int line, int segment)
			{
				Positions.Add(new Position(line, segment));
			}
		}

		public class LanguagePackInfo
		{
			public string Author;

			public string Code;

			public string Title;

			public string Appli;

			public string Version;

			public string Scale;
		}

		public const int JAPANESE = 0;

		public const int ENGLISH = 1;

		protected static int language_ = -1;

		protected static int priorityLanguage_ = -1;

		private static string KAIRO_JAPANESE = "*Japanese";

		private static string KAIRO_ENGLISH = "*English";

		public const int FORCERESET_OFF = 0;

		public const int FORCERESET_ON_JP_DEFAULT = 1;

		public static int forceResetType_ = 0;

		protected static readonly string[] FOLDERS = new string[2] { "Japanese.lproj", "English.lproj" };

		protected static string[][] softlabelTable_;

		protected static string[][][] textTable_;

		private static string[][] BUILTIN_TRANSLATE_TABLE = new string[298][]
		{
			new string[2] { "画面が広くなって<br>さらに<co=60,100,200>横回転</co>も<br>可能になりました!", "<po=c>The display area stream now larger and can also be viewed stream <co=3366ff>landscape mode</co>!" },
			new string[2] { "無事に広告表示が解除されたようですね！", "You've successfully removed ads from the game!" },
			new string[2] { "さらに、画面の回転に対応しました!!ﾒﾆｭｰ｢ｼｽﾃﾑ｣-｢設定変更｣から横回転のON/OFFが切り替えられます。", "You can also rotate the display if your device supports it! Toggle this on or off via \"System\" > \"Options.\"" },
			new string[2] { "画面も変わって新しい気分で<br>経営を進めていきましょう!", "Maybe the new display will offer fresh perspectives on the game! Enjoy!" },
			new string[2] { "通信を許可し電波を確認して再起動してください。\n通信OFFの場合は一部の機能がご利用頂けない場合がございます。", "Please ensure connections are enabled, check your signal, and restart the app. Some features may be inaccessible with connections disabled." },
			new string[2] { "メッセージ", "Message" },
			new string[2] { "ﾒｯｾｰｼﾞ", "Message" },
			new string[2] { "通信中", "Transmitting" },
			new string[2] { "了解", "OK" },
			new string[2] { "いらっしゃいませ", "Welcome!" },
			new string[2] { "売り切れ", "Sold Out" },
			new string[2] { "ﾌﾌﾌﾌ。\nどれにします", "Heh heh. Which do you want?" },
			new string[2] { "通信に失敗しました", "A communication error occurred." },
			new string[2] { "アイテムの購入", "Buy Items" },
			new string[2] { "利用規約を読む(必読)", "Read Terms of Use (Required)" },
			new string[2] { "規約に同意してﾏｰｹｯﾄへ移動", "Accept terms and proceed." },
			new string[2] { "購入後ｹﾞｰﾑを再開するとｹﾞｯﾄ", "Restart after purchase to claim items." },
			new string[2] { "買いに行く", "Proceed" },
			new string[2] { "規約に同意して買いに行く", "Accept terms and proceed" },
			new string[2] { "報告", "Alert" },
			new string[2] { "アイテムが無事に\n届きました", "Your purchase is here!" },
			new string[2] { "どちらに入店しますか", "Select a Shop" },
			new string[2] { "パンプキン商会", "Pumpkin Products" },
			new string[2] { "チケットショップ", "Ticket Shop" },
			new string[2] { "おっと…<br>本部との<co=60,100,200>通信に失敗</co>してしまいました。", "Oh, dear... We <co=60,100,200>can't seem to contact</co> corporate HQ." },
			new string[2] { "電波や通信の状態を確認して<br>もう一回お店に来ると…", "Come back to the shop after checking your signal and communications status." },
			new string[2] { "本来のステキな商品が陳列されますよ。", "Our usual impressive selection of products should be available to you then." },
			new string[2] { "というわけで…<br>またお待ちしています。", "We'll be waiting!" },
			new string[2] { "ここは、世界的に見て超レアな<br>｢チケット｣を売るお店ですよ。", "Here we sell tickets--premium items not easily found anywhere else stream the world." },
			new string[2] { "なかなかの金額だからよく考えて買っていってくださいね。", "They're a bit pricey, so think carefully before buying, okay?" },
			new string[2] { "フフフ。", "Heh heh." },
			new string[2] { "フフフ<br>いらっしゃい。", "Welcome!" },
			new string[2] { "あー、残念ながら<br>そちらは<co=60,100,200>売り切れ</co>です。<br>他の商品もご覧下さいませ。", "Ah, unfortunately we're <co=60,100,200>sold out</co> of that, but please have a look at our other offerings." },
			new string[2] { "チケットが最大の<0>枚を超えてしまいます。", "You can't exceed <0> tickets." },
			new string[2] { "おや、お目が高いですね。", "Ah, an excellent choice." },
			new string[2] { "こちらは<co=60,100,200>現実のお金で購入する</co>ステキな商品です。", "This wonderful item can only be purchased with <co=60,100,200>actual, real-life money</co>." },
			new string[2] { "ちびっこは、<co=60,100,200>保護者の方</co>に確認してから購入するようにして下さいね。フフフ。", "Children must check with a <co=60,100,200>parent or guardian</co> before making purchases." },
			new string[2] { "これからいつでもお店はありますからいつでも来て下さいね。", "This shop will be open at all times now, so stop by anytime." },
			new string[2] { "フフフ<br>また来てくださいね。", "Come again!" },
			new string[2] { "<0>さん", "<0>" },
			new string[2] { "ﾌﾟﾚｲ <0>時間<1>分", "Play time: <0> hr <1> min" },
			new string[2] { "申請してみる", "Friend Request" },
			new string[2] { "全国に散らばるユーザーを<br>フレンドとして最大<0>人まで<br>登録できます。<br>フレンドの合計ﾌﾟﾚｲ時間に<br>よって起動時にボーナスが<br>入ります。IDを聞いて友達<br>に申請してみましょう。", "You can register up to five<br>friends. You'll earn bonuses<br>at game startup based on their<br>total play time. Ask friends for<br>their IDs to add them." },
			new string[2] { "ﾆｯｸﾈｰﾑ", "Nickname" },
			new string[2] { "ﾆｯｸﾈｰﾑ ", "nickname" },
			new string[2] { "ﾆｯｸﾈｰﾑを入力してください", "Enter a nickname." },
			new string[2] { "空白以外で入力してください", "Nickname cannot include spaces." },
			new string[2] { "利用できない文字があります", "Contains one or more unusable characters." },
			new string[2] { "フレンドのID", "Friend's ID" },
			new string[2] { "フレンドのID ", "friend's ID" },
			new string[2] { "フレンドIDを入力してください", "Enter your friend's ID." },
			new string[2] { "おめでとうございます!<br>新しい<co=60,100,200>フレンド</co>が登録されボーナスが入りました!", "Congratulations! You get a bonus for registering a new <co=60,100,200>friend</co>!" },
			new string[2] { "一緒にプレイしていれば今後もボーナスが入っていきますよ!", "If you both continue playing, you'll earn more bonuses stream the future!" },
			new string[2] { "ちなみに<co=255,0,0>アプリは削除しないで</co>下さいね。削除するとフレンドが全部消えて戻せなくなってしまいます。", "By the way, <co=255,0,0>don't delete this app</co> or all of your friend data will be lost for good." },
			new string[2] { "ゲームの前に", "Getting Started" },
			new string[2] { "あなたのﾆｯｸﾈｰﾑを入力してね", "Please enter your nickname." },
			new string[2] { "ここをタッチ", "Tap here" },
			new string[2] { "※あとで変更できます※", "It can be changed later." },
			new string[2] { "OK", "OK" },
			new string[2] { "新しいﾆｯｸﾈｰﾑを入力してね", "Enter a new nickname." },
			new string[2] { "<0>  <1>時間 <2>分", "<0>  <1> hr <2> min" },
			new string[2] { "前回起動時", "Last Played" },
			new string[2] { "累計", "Total:" },
			new string[2] { "<0>時間<1>分", "<0> hr <1> min" },
			new string[2] { "から申請があります。", "has sent a friend request." },
			new string[2] { "他<0>件", "<0> other(s)" },
			new string[2] { "フレンドを探す", "Find a friend" },
			new string[2] { "ﾌﾚﾝﾄﾞ説明", "Help" },
			new string[2] { "IDを教える", "Share ID" },
			new string[2] { "あなたのIDはこちら(ﾀｯﾁしてね)", "Your ID (tap to share)" },
			new string[2] { "検索はこちら", "Search here" },
			new string[2] { "ここに入力", "Enter here" },
			new string[2] { "検索", "Search" },
			new string[2] { "確認", "Message" },
			new string[2] { "承認まで時間がかかります", "Approval may take time." },
			new string[2] { "申請する", "Request" },
			new string[2] { "やめる", "Cancel" },
			new string[2] { "に申請しました。", "Friend request sent to" },
			new string[2] { "許可されるまでお待ちください", "Please wait for approval." },
			new string[2] { "友達に知らせよう", "Message" },
			new string[2] { "閉じる", "Close" },
			new string[2] { "反映まで時間がかかります", "Updates may take time." },
			new string[2] { "承認する", "Approve" },
			new string[2] { "拒否する", "Decline" },
			new string[2] { "フレンドの承認", "Friend Added" },
			new string[2] { "フレンド増加", "New Friend" },
			new string[2] { "<co=0070FF><0>さん</co><br>を承認しました。", "<co=0070FF><0></co><br>added." },
			new string[2] { "ｹﾞｰﾑを進めるとﾎﾞｰﾅｽがあるかも", "You may receive bonuses." },
			new string[2] { "※相手には通知されません※", "The player will not be notified." },
			new string[2] { "削除する", "Delete" },
			new string[2] { "もどる", "Back" },
			new string[2] { "<0>さん\nを削除しますか?", "Delete<br><0>?" },
			new string[2] { "フレンドについて", "About Friends" },
			new string[2] { "ﾌﾚﾝﾄﾞﾎﾞｰﾅｽ(<0>分毎)", "Friend Bonus (every <0> min)" },
			new string[2] { "ﾌﾚﾝﾄﾞから <co=254,233,1><0></co>時間<co=254,233,1><1></co>分 取得", "You got a friend bonus of <co=254,233,1><0></co> hr <co=254,233,1><1></co> min." },
			new string[2] { "(最大)", "(Max)" },
			new string[2] { "受け取る", "Claim" },
			new string[2] { "また次回", "Next Time" },
			new string[2] { "ﾌﾚﾝﾄﾞﾎﾞｰﾅｽGET", "Friend Bonus" },
			new string[2] { "<co=0070FF>チケット<0>枚</co><br>入手できました。", "You got<br><co=0070FF><0> tickets</co>." },
			new string[2] { "ﾌﾚﾝﾄﾞﾎﾞｰﾅｽ", "Friend Bonus" },
			new string[2] { "フレンドが<co=254,233,1><0>人</co>増えました!", "New friends: <co=254,233,1><0></co>" },
			new string[2] { "<co=0,128,255><0></co>さんが<br>ﾌﾚﾝﾄﾞになりました!", "You've added<br><co=0,128,255><0></co>!" },
			new string[2] { "通信できませんでした。|設定を確認し時間を|おいて挑戦して下さい。&ﾌﾚﾝﾄﾞは<friend_max>人まで登録できます", "Transmission failed. Check your settings and try again later." },
			new string[2] { "サポート専用\nｼｮｯﾌﾟｺｰﾄﾞを入力して下さい", "Please enter the shop code (for customer support only)." },
			new string[2] { "ｼｮｯﾌﾟｺｰﾄﾞ", "Shop Code" },
			new string[2] { "セーブデータを受信する", "Receive save data" },
			new string[2] { "サポート専用\nお問合せｺｰﾄﾞを入力して下さい", "Please enter your\ninquiry support code." },
			new string[2] { "お問合せｺｰﾄﾞ", "Inquiry support code" },
			new string[2] { "オススメアプリ情報", "Recommended Apps" },
			new string[2] { "タイトル画面へ", "Go to Title Screen" },
			new string[2] { "お友達にこのアプリを紹介", "Invite your friends!" },
			new string[2] { "0Pは登録できません。", "You can't register 0 points." },
			new string[2] { "<0>は登録できません。", "You can't register <0>." },
			new string[2] { "ｱﾌﾟﾘのﾊﾞｰｼﾞｮﾝｱｯﾌﾟを行います", "Updating app..." },
			new string[2] { "認証に失敗しました。購入時のアカウントを設定し、電波状態を確認してください。", "Authentication failed. Please use the account you used to purchase the app and check your signal." },
			new string[2] { "購入時のアカウントを設定し、電波状態を確認してください。", "Please use the account you used to purchase the app and check your signal." },
			new string[2] { "横画面モードは", "Landscape mode available" },
			new string[2] { "正式版でご利用頂けます!!", "in the Full Version!" },
			new string[2] { "システムの都合上 <0> は使えません", "Do not use: <0>" },
			new string[2] { "<0>を入力して下さい", "Please enter a <0>." },
			new string[2] { "ｱﾌﾟﾘ起動中", "Loading" },
			new string[2] { "リソース読込失敗", "Loading failed" },
			new string[2] { "スタート", "Start" },
			new string[2] { "セーブスロット", "Slot" },
			new string[2] { "中断データから再開", "Resume" },
			new string[2] { "セーブデータを読込", "Load Data" },
			new string[2] { "中断データなし", "No autosave data" },
			new string[2] { "空きスロット", "Empty" },
			new string[2] { "<0>年", "Yr <0>" },
			new string[2] { "<0>月 <1>週", "Mn <0> Wk <1>" },
			new string[2] { "システムの都合上 / | < > \\ は使えません", "Do not use: / | < > \\" },
			new string[2] { "データを選択してください", "Select data to load" },
			new string[2] { "セーブデータがありません", "No save data found" },
			new string[2] { "このゲームについて", "About the Game" },
			new string[2] { "最高クリアポイント", "Max Clear Points" },
			new string[2] { "記録がありません", "None" },
			new string[2] { "該当\u3000無し", "None" },
			new string[2] { "正式版へ", "Full Ver." },
			new string[2] { "ハイスコアは<br>正式版でご利用頂けます!!", "Get the full version to upload your hi scores!" },
			new string[2] { "おつかれさまでした。<br>ここでLite版は終了となります", "Well done. You've finished the Lite version." },
			new string[2] { "このあと正式版のサイトにアクセスすることが出来ます。", "After this you'll jump to our site, so check it out!" },
			new string[2] { "それではまた会いましょう。", "Thanks for playing!" },
			new string[2] { "<0>最新号<br>が発行されました", "The latest <0><br>is now on sale." },
			new string[2] { "はい", "Yes" },
			new string[2] { "いいえ", "No" },
			new string[2] { "ｼｽﾃﾑ", "System" },
			new string[2] { "セーブ", "Save" },
			new string[2] { "設定変更", "Options" },
			new string[2] { "遊び方", "Help" },
			new string[2] { "ハイスコア", "High Score" },
			new string[2] { "ｹﾞｰﾑ終了", "Quit" },
			new string[2] { "続きから", "Continue" },
			new string[2] { "最初から", "New Game" },
			new string[2] { "建築", "Build" },
			new string[2] { "情報", "Info" },
			new string[2] { "ｱｲﾃﾑ", "Items" },
			new string[2] { "ﾀｲﾄﾙへ", "Title" },
			new string[2] { "小計 <0>P", "Subtotal: <0>" },
			new string[2] { "合計 <0>P !!", "Total: <0>" },
			new string[2] { "<0>個", "<0>" },
			new string[2] { "<0>種", "<0>" },
			new string[2] { "<0>人", "<0>" },
			new string[2] { "<0>P", "<0>" },
			new string[2] { "<0>P !!", "<0>!!" },
			new string[2] { "ゲームセーブ中", "Saving game..." },
			new string[2] { "会話ウィンドウ", "Speech Window" },
			new string[2] { "イベント", "Event" },
			new string[2] { "設定", "Options" },
			new string[2] { "画面", "Priority" },
			new string[2] { "ｽﾋﾟｰﾄﾞ", "Speed" },
			new string[2] { "BGM", "Music" },
			new string[2] { "効果音", "SFX" },
			new string[2] { "画面回転", "Rotation" },
			new string[2] { "描画優先", "Graphics" },
			new string[2] { "速度優先", "Perf." },
			new string[2] { "高速", "Fast" },
			new string[2] { "普通", "Normal" },
			new string[2] { "自動", "Auto" },
			new string[2] { "固定", "Lock" },
			new string[2] { "速度優先にすると一部の演出や", "Selecting “Perf.” prioritizes" },
			new string[2] { "表示が削除されます", "performance over graphics." },
			new string[2] { "ゲームの動作速度を変更します", "Change the game's speed." },
			new string[2] { "ｹﾞｰﾑをｸﾘｱすると変更できます", "(Unlocked after beating game.)" },
			new string[2] { "本体の向きに合わせて", "Choose to automatically" },
			new string[2] { "画面を回転するか設定します", "rotate the screen or not." },
			new string[2] { "ゲーム中のボリュームを", "Adjust the volume." },
			new string[2] { "設定します", "" },
			new string[2] { "for カイロパーク", "(C) Kairosoft Co., Ltd." },
			new string[2] { "(C)カイロソフト", "(C) Kairosoft Co., Ltd." },
			new string[2] { "クリアポイント", "Clear Points" },
			new string[2] { "ｸﾘｱﾎﾟｲﾝﾄの計算", "Page" },
			new string[2] { "= + <0>P", "= <0>" },
			new string[2] { "次へ", "Next" },
			new string[2] { "維持/月", "Maint." },
			new string[2] { "基本", "Base" },
			new string[2] { "ﾎﾞｰﾅｽ", "Bns" },
			new string[2] { "合計", "Total" },
			new string[2] { "ボーナス", "Bonus" },
			new string[2] { "男性", "Male" },
			new string[2] { "女性", "Female" },
			new string[2] { "<0>月", "Month <0>" },
			new string[2] { "維持費", "Maint." },
			new string[2] { "<0>/月", "<0>/month" },
			new string[2] { "未設定", "None" },
			new string[2] { "価格", "Price" },
			new string[2] { "在庫", "Stock" },
			new string[2] { "所有数", "Owned" },
			new string[2] { "完了", "Complete" },
			new string[2] { "実行中", "In progress" },
			new string[2] { "<0>個 完了", "<0> complete" },
			new string[2] { "<0>\u3000<1>月号", "<0>: <1> Issue" },
			new string[2] { "アイテム一覧", "Item List" },
			new string[2] { "所有", "Owned" },
			new string[2] { "アイテムの一覧", "Item List" },
			new string[2] { "使用回数", "Times Used" },
			new string[2] { "<0> 回", "<0>" },
			new string[2] { "『<0>』", "<0>" },
			new string[2] { "概要", "Summary" },
			new string[2] { "期間", "Length" },
			new string[2] { "費用", "Cost" },
			new string[2] { "名前", "Name" },
			new string[2] { "順位", "Rank" },
			new string[2] { "施設の一覧", "Facilities" },
			new string[2] { "全て", "All" },
			new string[2] { "全ての施設", "All Facilities" },
			new string[2] { "一覧", "List" },
			new string[2] { "今のセーブデータに<br>上書きされます", "Overwrite current progress?" },
			new string[2] { "今のセーブデータは<br>消えてしまいます", "Delete current progress?" },
			new string[2] { "ゲームセーブ完了", "Save complete." },
			new string[2] { "画面をスクロールしながら建てる場所を選びます。建物をタッチしながら動かせば細かい調整もできます。", "Scroll to select a location. Keep your finger on the item while scrolling to make fine adjustments. Tap to build." },
			new string[2] { "既に設定されています", "Already set." },
			new string[2] { "お金が足りません", "You don't have enough money." },
			new string[2] { "臨時収入 <co=000080><0></co>を入手しました", "You got <co=000080><0></co>." },
			new string[2] { "人の追跡を開始します。<br>このあと、もう一回人をタッチすると詳しい情報も見れます。", "Now following this guest. Tap again to see detailed info." },
			new string[2] { "<0>を建築中です", "Building <0>..." },
			new string[2] { "『<0>』を撤去", "Remove <0>" },
			new string[2] { "どこまで撤去しますか？", "Remove until where?" },
			new string[2] { "どこから<0>を敷きますか？", "Start <0> where?" },
			new string[2] { "どこまで敷きますか？", "Place until where?" },
			new string[2] { "春", "Spr" },
			new string[2] { "夏", "Sum" },
			new string[2] { "秋", "Aut" },
			new string[2] { "冬", "Wnt" },
			new string[2] { "『<co=008000><0></co>』が完了", "<co=008000><0></co> is complete." },
			new string[2] { "維持費が<0> ひかれました", "-<0> in monthly maintenance." },
			new string[2] { "カイロニュース", "Kairo News" },
			new string[2] { "カイロパークに飛びます", "Visit us at Kairo Park." },
			new string[2] { "アプリ紹介ページに飛びます", "Check out Kairo Park Apps." },
			new string[2] { "ゲームを終了して", "Quit the game and head to" },
			new string[2] { "カイロパークへ接続します", "Head to Kairo Park?" },
			new string[2] { "アプリ紹介ページへ接続します", "Head to our App intro page?" },
			new string[2] { "サイトへ行く", "OK" },
			new string[2] { "ゲームを終了します", "Quit the game?" },
			new string[2] { "最新ゲームの情報は", "Stay up-to-date with the" },
			new string[2] { "カイロパークでGET", "latest Kairo Park info!" },
			new string[2] { "通貨単位", "Currency" },
			new string[2] { "円", "\\" },
			new string[2] { "ドル", "$" },
			new string[2] { "お金の表記を変更できます", "Switch currency to yen/dollars." },
			new string[2] { "\\100 = $1というレートです", "Note that ￥100 = ＄1." },
			new string[2] { "カイロパークはこちら", "Enter Kairo Park" },
			new string[2] { "認証中", "Loading" },
			new string[2] { "名前を入力して下さい", "Please enter a name." },
			new string[2] { "全角<0>文字以内にして下さい", "Enter up to <0> characters." },
			new string[2] { "<0>年<1>月<2>週", "Yr <0> Mn <1> Wk <2>" },
			new string[2] { "ロードに失敗しました", "Load failed." },
			new string[2] { "読み込みに失敗しました。", "Could not read data." },
			new string[2] { "通信を許可し電波状態を確認して下さい。", "Make sure to allow connection to network and check your signal strength." },
			new string[2] { "通信を許可し電波状況<br>を確認して下さい", "Make sure to allow connection to network and<br>check your signal quality." },
			new string[2] { "認証に失敗しました。", "Authentication failed." },
			new string[2] { "サーバーエラーが発生しました。", "A server error has occurred." },
			new string[2] { "データエラーが発生しました。<br>アプリを再インストールしてください。", "A data error has occurred.<br>Please re-install the application." },
			new string[2] { "起動失敗 アプリを再起動して下さい", "An error has occurred.<br>Please restart the application." },
			new string[2] { "ゲーム開始に失敗しました", "Could not start game." },
			new string[2] { "中断ﾃﾞｰﾀ<0>", "Auto <0>" },
			new string[2] { "ｾｰﾌﾞﾃﾞｰﾀ<0>", "Save <0>" },
			new string[2] { "全てのﾃﾞｰﾀを送信", "Send all data" },
			new string[2] { "エラー内容を表示する", "Show error log" },
			new string[2] { "全てのデータを送信します", "Send all data?" },
			new string[2] { "中断データ<0> を送信します", "Send Autosave Data <0>?" },
			new string[2] { "セーブデータ<0> を送信します", "Send Save Data <0>?" },
			new string[2] { "送信を完了しました。<br>ご協力ありがとうございます。<br>今後もカイロパークを宜しくお願い致します。", "Data sent.<br>Thanks for your cooperation.<br>We hope you'll stick with us!" },
			new string[2] { "エラーが発生しました<br>リトライしますか？", "An error has occurred.<br>Retry?" },
			new string[2] { "セーブデータの送信", "Send Save Data" },
			new string[2] { "送る内容を選んでください", "Choose data to send." },
			new string[2] { "データの送信中", "Sending data" },
			new string[2] { "電源キーを押さないでください。", "Do not turn the device off." },
			new string[2] { "ｴﾗｰ", "Error" },
			new string[2] { "Lite版", "Lite Version" },
			new string[2] { "エラーが発生しました。", "An error has occurred." },
			new string[2] { "決定", "OK" },
			new string[2] { "アプリの設定画面から「アプリの権限」をすべてオンにして再起動して下さい。", "On the app settings screen, turn on all \"App Permissions\" and restart the app." },
			new string[2] { "しばらくお待ちください", "Please wait..." },
			new string[2] { "権限：ストレージを許可して起動し直してください。<br>※ゲームの保存に必要となります。", "Authority: Please re-boot to allow the storage.<br>You will need to save the game." },
			new string[2] { "起動できません。", "You can not start." },
			new string[2] { "内容を短くして下さい", "Please shorten the content" },
			new string[2] { "Tapjoyの機能をご利用する際には\n追跡型広告制限を解除し、アプリを再起動してください。\n設定アプリ > プライバシー > 広告 > 追跡型広告制限", "How to use TapJoy\n- Terminate \"Limit Ad Tracking\" and restart the game.\n* Setting app > Privacy > Advertising > Limit Ad Tracking" }
		};

		private static string[][] BUILTIN_SOFTLABEL_TABLE = new string[93][]
		{
			new string[2] { "決定", "Confirm" },
			new string[2] { "終了", "Finish" },
			new string[2] { "戻る", "Back" },
			new string[2] { "はい", "Yes" },
			new string[2] { "いいえ", "No" },
			new string[2] { "確認", "Check" },
			new string[2] { "セーブ", "Save" },
			new string[2] { "メニュー", "Menu" },
			new string[2] { "MAP", "Map" },
			new string[2] { "通常", "Normal" },
			new string[2] { "出発", "Depart" },
			new string[2] { "全て", "All" },
			new string[2] { "完了", "Complete" },
			new string[2] { "HELP", "Help" },
			new string[2] { "自分", "Self" },
			new string[2] { "上位", "Top" },
			new string[2] { "ｻｲﾄ", "Site" },
			new string[2] { "価格", "Price" },
			new string[2] { "人気", "Popular" },
			new string[2] { "開始", "Start" },
			new string[2] { "編集", "Edit" },
			new string[2] { "相性", "Compat." },
			new string[2] { "回転", "Rotate" },
			new string[2] { "称号", "Name" },
			new string[2] { "次へ", "Next" },
			new string[2] { "報告書", "Report" },
			new string[2] { "輸出", "Export" },
			new string[2] { "自動", "Auto" },
			new string[2] { "ヒント", "Hint" },
			new string[2] { "順番", "Order" },
			new string[2] { "捕捉", "Note" },
			new string[2] { "初期化", "Defaults" },
			new string[2] { "退却", "Remove" },
			new string[2] { "高速", "Fast" },
			new string[2] { "標準", "Normal" },
			new string[2] { "解除", "Cancel" },
			new string[2] { "販売", "Sell" },
			new string[2] { "改名", "Rename" },
			new string[2] { "詳細", "Details" },
			new string[2] { "辞典", "Guide" },
			new string[2] { "反転", "Rotate" },
			new string[2] { "中止", "Cancel" },
			new string[2] { "階層", "Floor" },
			new string[2] { "説明", "Info" },
			new string[2] { "お任せ", "Auto" },
			new string[2] { "切替", "Toggle" },
			new string[2] { "外す", "Remove" },
			new string[2] { "省略", "Skip" },
			new string[2] { "地図", "Map" },
			new string[2] { "ｽﾃｰﾀｽ", "Status" },
			new string[2] { "部隊", "Units" },
			new string[2] { "棄権", "Forfeit" },
			new string[2] { "作戦", "Tactics" },
			new string[2] { "ｿｰﾄ", "Sort" },
			new string[2] { "診断", "Diagnose" },
			new string[2] { "ｾｰﾌﾞ", "Save" },
			new string[2] { "ﾒﾆｭｰ", "Menu" },
			new string[2] { "補足", "Supplement" },
			new string[2] { "設定", "Setting" },
			new string[2] { "下へ", "Down" },
			new string[2] { "上へ", "To top" },
			new string[2] { "清算", "Liquidation" },
			new string[2] { "合成", "Synthesis" },
			new string[2] { "仲間", "Fellow" },
			new string[2] { "追跡", "Tracking" },
			new string[2] { "最大", "Maximum" },
			new string[2] { "褒美", "Reward" },
			new string[2] { "船へ", "To ship" },
			new string[2] { "牢屋", "Jail" },
			new string[2] { "ｼﾞｮﾌﾞ", "Job" },
			new string[2] { "ﾊﾟｽﾜｰﾄﾞ", "Password" },
			new string[2] { "GET!!", "GET !!" },
			new string[2] { "ｵｽｽﾒ", "Recommended" },
			new string[2] { "閉じる", "Close up" },
			new string[2] { "行動", "Action" },
			new string[2] { "倉庫", "Warehouse" },
			new string[2] { "効果", "Effect" },
			new string[2] { "同意", "Consent" },
			new string[2] { "規約", "Agreement" },
			new string[2] { "ｼｬｯﾌﾙ", "Shuffle" },
			new string[2] { "停止", "Stop" },
			new string[2] { "履歴", "History" },
			new string[2] { "投入", "Input" },
			new string[2] { "完成", "Completion" },
			new string[2] { "解雇", "Dismissal" },
			new string[2] { "ｽｷﾙ", "Skill" },
			new string[2] { "ﾊﾟﾝﾌﾟｷﾝ", "Pumpkin" },
			new string[2] { "撤去", "Removal" },
			new string[2] { "購入", "Purchase" },
			new string[2] { "売却", "Sale" },
			new string[2] { "LVUP", "LVUP" },
			new string[2] { "ｽﾞｰﾑ", "Zoom" },
			new string[2] { "情報", "Information" }
		};

		private static string[][][] builtInTextTable_;

		private static LangPackRecord[] langPackRecords_;

		private static Dictionary<string, List<RecordPosition>[]> langPackLines_;

		private static Dictionary<string, LangPackFormat> langPackFormats_;

		private static string langPackAuthor_;

		private static string langPackCode_;

		private static string langPackTitle_;

		private static int langPackScale_;

		private static string[] langPackFolders_;

		private static string langPackFile_;

		private static string[][] systemLangPackFiles_;

		public const string PREF_LANG_PACK = "_language_pack";

		private const string PREF_CUSTOM_LANG_LOCATION = "_language_pack_location";

		private const string PREF_CUSTOM_LANG_SEND = "_language_pack_send";

		public const string CUSTOM_LANG_FILE = "_custom_lang_pack.csv";

		public static void Init()
		{
			if (systemLangPackFiles_ == null)
			{
				string filename = "language.dat";
				List<string[]> list = new List<string[]>();
				if (Config.LANGUAGE == -1)
				{
					if (English())
					{
						list.Add(new string[3] { "English", "", "en-US" });
						list.Add(new string[3] { "日本語", KAIRO_JAPANESE, "" });
					}
					else
					{
						list.Add(new string[3] { "日本語", "", "ja-JP" });
						list.Add(new string[3] { "English", KAIRO_ENGLISH, "" });
					}
				}
				else
				{
					list.Add(new string[3] { "English", "", "en-US" });
				}
				JarInflater jarInflater = null;
				try
				{
					if (Storage.Exists(3, filename))
					{
						jarInflater = new JarInflater(Storage.Read(3, false, filename));
						string[] files = jarInflater.GetFiles();
						foreach (string text in files)
						{
							try
							{
								LanguagePackInfo languagePackInfo = GetLanguagePackInfo(jarInflater.GetData(text));
								list.Add(new string[3] { languagePackInfo.Title, text, languagePackInfo.Code });
							}
							catch
							{
							}
						}
					}
				}
				catch
				{
				}
				finally
				{
					if (((object)(jarInflater) != null)) { (jarInflater).Close(); }
					jarInflater = null;
				}
				systemLangPackFiles_ = list.ToArray();
			}
			if (forceResetType_ != 0 && PlayerPrefs.HasKey("_language_pack"))
			{
				PlayerPrefs.DeleteKey("_language_pack");
				if (Config.PRINT)
				{
					Log.Info("PlayerPrefs.DeleteKey(PREF_LANG_PACK)");
				}
			}
			if (!PlayerPrefs.HasKey("_language_pack") && Config.LANGUAGE != 0)
			{
				JLocale jLocale = JLocale.GetDefault();
				string text2 = jLocale.GetLanguage().ToLower();
				string text3 = jLocale.GetCountry().ToLower();
				if (text2 == "zh")
				{
					if (text3 == "sg")
					{
						text3 = "ch";
					}
					if (text3 == "hk")
					{
						text3 = "tw";
					}
					if (text3 == "mo")
					{
						text3 = "tw";
					}
				}
				string[] array = null;
				string[] array2 = null;
				string[][] array3 = systemLangPackFiles_;
				foreach (string[] array4 in array3)
				{
					string text4 = array4[2].ToLower();
					if (text2 + "-" + text3 == text4)
					{
						array = array4;
					}
					if (text2 == text4.Split('-')[0])
					{
						array2 = array4;
					}
				}
				if (forceResetType_ == 1)
				{
					if (Config.PRINT)
					{
						if (array == null)
						{
							Log.Info("bestLangPack == null");
						}
						else
						{
							Log.Info("bestLangPack[1]" + array[1]);
						}
						if (array2 == null)
						{
							Log.Info("matchLangPack == null");
						}
						else
						{
							Log.Info("matchLangPack[1]" + array2[1]);
						}
					}
					if ((array == null || array[1].Equals("")) && (array2 == null || array2[1].Equals("")))
					{
						array = new string[3] { "日本語", KAIRO_JAPANESE, "" };
					}
				}
				if (array != null)
				{
					PlayerPrefs.SetString("_language_pack", array[1]);
				}
				else if (array2 != null)
				{
					PlayerPrefs.SetString("_language_pack", array2[1]);
				}
			}
			byte[] array5 = Storage.Read(3, false, "language_pack_format.txt");
			if (array5 != null)
			{
				string text5 = StringUtil.GetString(array5);
				langPackFormats_ = new Dictionary<string, LangPackFormat>();
				foreach (string recoveredLangFormatLine in text5.Split('\n'))
				{
					string[] array6 = recoveredLangFormatLine.Split(',');
					LangPackFormat langPackFormat = new LangPackFormat(array6[0], (char)int.Parse(array6[1]), (char)int.Parse(array6[2]));
					langPackFormats_[langPackFormat.File.ToLower()] = langPackFormat;
				}
				if (!SetLanguagePack(PlayerPrefs.GetString("_language_pack", null), null, false))
				{
					SetLanguagePack(null);
				}
			}
		}

		public static int Get()
		{
			if (priorityLanguage_ != -1)
			{
				return priorityLanguage_;
			}
			if (language_ != -1)
			{
				return language_;
			}
			if (Config.LANGUAGE == -1)
			{
				language_ = Get(JLocale.GetDefault());
			}
			else
			{
				language_ = Config.LANGUAGE;
			}
			return language_;
		}

		public static int Get(JLocale locale)
		{
			if (priorityLanguage_ != -1)
			{
				return priorityLanguage_;
			}
			if (JLocale.JAPAN.Equals(locale) || JLocale.JAPANESE.Equals(locale))
			{
				return 0;
			}
			return 1;
		}

		public static bool Japanese()
		{
			return Get() == 0;
		}

		public static bool English()
		{
			return Get() == 1;
		}

		public static bool IsJapaneseStyleCalendar()
		{
			string languageCode = GetLanguageCode();
			if (languageCode.StartsWith("ja") || languageCode.StartsWith("zh") || languageCode.StartsWith("ko"))
			{
				return true;
			}
			return false;
		}

		public static void SetSoftLabelTable(string[][] softlabelTable)
		{
			Language.softlabelTable_ = softlabelTable;
			if (Language.softlabelTable_ == null)
			{
				return;
			}
			for (int i = 0; i < (int)Language.softlabelTable_.Length; i++)
			{
				Language.softlabelTable_[i][0] = StringUtil.Replace(Language.softlabelTable_[i][0], "\\", "¥");
				Language.softlabelTable_[i][1] = StringUtil.Replace(Language.softlabelTable_[i][1], "\\", "¥");
			}
		}

		public static void SetTextTable(string[][] textTable)
		{
			Language.textTable_ = null;
			if (textTable == null)
			{
				return;
			}
			Language.textTable_ = Language.MakeTextTable(textTable);
		}

		public static void ClearSoftLabelTable()
		{
			softlabelTable_ = null;
		}

		public static void ClearTextTable()
		{
			textTable_ = null;
		}

		private static string[][][] MakeTextTable(string[][] textTable)
		{
			string[][] array = textTable;
			foreach (string[] array2 in array)
			{
				string text = array2[0];
				MatchCollection matchCollection = Regex.Matches(text, "<co=([0-9A-F]+)>");
				if (matchCollection.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int j = 0; j < matchCollection.Count; j++)
					{
						Match match = matchCollection[j];
						if (j == 0)
						{
							stringBuilder.Append(text.Substring(0, match.Index));
						}
						stringBuilder.Append("<co=" + match.Groups[1].Value.ToLower() + ">");
						int num = text.Length;
						if (j + 1 < matchCollection.Count)
						{
							num = matchCollection[j + 1].Index;
						}
						stringBuilder.Append(text.Substring(match.Index + match.Length, num - (match.Index + match.Length)));
					}
					text = stringBuilder.ToString();
				}
				matchCollection = Regex.Matches(text, "<co=([0-9\\s]+),([0-9\\s]+),([0-9\\s]+)>");
				if (matchCollection.Count > 0)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					for (int k = 0; k < matchCollection.Count; k++)
					{
						Match match2 = matchCollection[k];
						if (k == 0)
						{
							stringBuilder2.Append(text.Substring(0, match2.Index));
						}
						string text2 = $"{int.Parse(match2.Groups[1].Value.Trim()):x2}{int.Parse(match2.Groups[2].Value.Trim()):x2}{int.Parse(match2.Groups[3].Value.Trim()):x2}";
						stringBuilder2.Append("<co=" + text2 + ">");
						int num2 = text.Length;
						if (k + 1 < matchCollection.Count)
						{
							num2 = matchCollection[k + 1].Index;
						}
						stringBuilder2.Append(text.Substring(match2.Index + match2.Length, num2 - (match2.Index + match2.Length)));
					}
					text = stringBuilder2.ToString();
				}
				array2[0] = text;
			}
			List<string[]> list = new List<string[]>();
			array = textTable;
			foreach (string[] array3 in array)
			{
				for (int l = 0; l < array3.Length; l++)
				{
					if (array3[l].StartsWith("<po="))
					{
						int num3 = array3[l].IndexOf('>');
						if (num3 != -1)
						{
							array3[l] = array3[l].Substring(num3 + 1);
						}
						if (l != 0)
						{
							array3[l] = AdjustMacroScope(array3[l]);
						}
					}
				}
				list.Add(array3);
				string text3 = array3[0];
				string src = text3;
				src = AdjustMacroScope(src);
				MatchCollection matchCollection2 = Regex.Matches(src, "<co=([0-9a-f]+)>");
				bool flag = matchCollection2.Count > 0;
				if (matchCollection2.Count == 0)
				{
					matchCollection2 = Regex.Matches(src, "<co=([0-9\\s]+),([0-9\\s]+),([0-9\\s]+)>");
				}
				if (matchCollection2.Count > 0)
				{
					StringBuilder stringBuilder3 = new StringBuilder();
					for (int m = 0; m < matchCollection2.Count; m++)
					{
						Match match3 = matchCollection2[m];
						if (m == 0)
						{
							stringBuilder3.Append(src.Substring(0, match3.Index));
						}
						string text4 = "";
						if (flag) { text4 = (int.Parse(match3.Groups[1].Value.Substring(0, 2), NumberStyles.HexNumber) + "," + int.Parse(match3.Groups[1].Value.Substring(2, 2), NumberStyles.HexNumber) + "," + int.Parse(match3.Groups[1].Value.Substring(4, 2), NumberStyles.HexNumber)); } else { text4 = $"{int.Parse(match3.Groups[1].Value.Trim()):X2}{int.Parse(match3.Groups[2].Value.Trim()):X2}{int.Parse(match3.Groups[3].Value.Trim()):X2}"; }
						stringBuilder3.Append("<co=" + text4 + ">");
						int num4 = src.Length;
						if (m + 1 < matchCollection2.Count)
						{
							num4 = matchCollection2[m + 1].Index;
						}
						stringBuilder3.Append(src.Substring(match3.Index + match3.Length, num4 - (match3.Index + match3.Length)));
					}
					src = stringBuilder3.ToString();
				}
				if (src != text3)
				{
					string[] array4 = new string[array3.Length];
					Array.Copy(array3, array4, array3.Length);
					array4[0] = src;
					list.Add(array4);
				}
				text3 = src;
				src = src.Replace("～", "〜");
				if (src != text3)
				{
					string[] array5 = new string[array3.Length];
					Array.Copy(array3, array5, array3.Length);
					array5[0] = src;
					list.Add(array5);
				}
			}
			int num5 = 0;
			for (int n = 0; n < list.Count; n++)
			{
				if (num5 < list[n][0].Length())
				{
					num5 = list[n][0].Length();
				}
			}
			num5++;
			Vector[] array6 = new Vector[num5];
			for (int num6 = 0; num6 < list.Count; num6++)
			{
				int num7 = list[num6][0].Length();
				if (array6[num7] == null)
				{
					array6[num7] = new Vector();
				}
				array6[num7].Add(list[num6]);
				if (!Japanese())
				{
					list[num6][0] = StringUtil.Replace(list[num6][0], "\\", "¥");
					list[num6][1] = StringUtil.Replace(list[num6][1], "\\", "¥");
				}
			}
			string[][][] array7 = new string[array6.Length][][];
			for (int num8 = 0; num8 < array7.Length; num8++)
			{
				if (!(array6[num8] == null))
				{
					array7[num8] = new string[array6[num8].Size()][];
					for (int num9 = 0; num9 < array7[num8].Length; num9++)
					{
						array7[num8][num9] = (string[])array6[num8].ElementAt(num9);
					}
				}
			}
			return array7;
		}

		private static string AdjustMacroScope(string src)
		{
			List<string> list = null;
			int startIndex = 0;
			while (true)
			{
				startIndex = src.IndexOf('<', startIndex);
				if (startIndex == -1)
				{
					break;
				}
				if (src.StartsWith("<co=", startIndex) || src.StartsWith("<si=", startIndex) || src.StartsWith("<st=", startIndex))
				{
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add("</" + src.Substring(startIndex + 1, 2) + ">");
				}
				else if ((src.StartsWith("</co>", startIndex) || src.StartsWith("</si>", startIndex) || src.StartsWith("</st>", startIndex)) && list != null)
				{
					string text = src.Substring(startIndex, 5);
					for (int num = list.Count - 1; num >= 0; num--)
					{
						if (list[num] == text)
						{
							list.RemoveAt(num);
							break;
						}
					}
				}
				startIndex++;
			}
			if (list != null && list.Count > 0)
			{
				string text2 = "";
				for (int i = 0; i < list.Count; i++)
				{
					text2 += list[i];
				}
				src += text2;
			}
			return src;
		}

		public static string TranslateSoftLabel(string softlabel)
		{
			if (softlabel == null || softlabel.Length > 10)
			{
				return softlabel;
			}
			if (softlabelTable_ != null)
			{
				for (int i = 0; i < softlabelTable_.Length; i++)
				{
					if (softlabel.Length == softlabelTable_[i][0].Length && softlabel.Equals(softlabelTable_[i][0]))
					{
						return softlabelTable_[i][1];
					}
				}
			}
			int num = Get();
			if (num != 0)
			{
				try
				{
					for (int j = 0; j < BUILTIN_SOFTLABEL_TABLE.Length; j++)
					{
						if (softlabel.Length == BUILTIN_SOFTLABEL_TABLE[j][0].Length && softlabel.Equals(BUILTIN_SOFTLABEL_TABLE[j][0]))
						{
							if (num >= BUILTIN_SOFTLABEL_TABLE[j].Length)
							{
								num = 1;
							}
							if (langPackRecords_ != null && BUILTIN_SOFTLABEL_TABLE[j].Length >= 3)
							{
								num = 2;
							}
							return BUILTIN_SOFTLABEL_TABLE[j][num];
						}
					}
				}
				catch
				{
				}
			}
			return softlabel;
		}

		public static string LT(string text, string arg = "")
		{
			return StringUtil.Replace(TranslateText(text), arg);
		}

		public static string LT(string text, long arg)
		{
			return LT(text, arg.ToString());
		}

		public static string LT(string text, Exception e)
		{
			return LT(text, e.ToString());
		}

		public static string TranslateText(string text, bool tabCheck = true)
		{
			text = StringUtil.Replace(text, "\\", "¥");
			if (text.IndexOf('\b') != -1)
			{
				text = StringUtil.Replace(text, "\b", "");
				return text;
			}
			if (text.IndexOf("<") != -1)
			{
				MatchCollection matchCollection = Regex.Matches(text, "<co=[0-9A-Z]+>");
				if (matchCollection.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < matchCollection.Count; i++)
					{
						Match match = matchCollection[i];
						if (i == 0)
						{
							stringBuilder.Append(text.Substring(0, match.Index));
						}
						stringBuilder.Append(match.Value.ToLower());
						int num = text.Length;
						if (i + 1 < matchCollection.Count)
						{
							num = matchCollection[i + 1].Index;
						}
						stringBuilder.Append(text.Substring(match.Index + match.Length, num - (match.Index + match.Length)));
					}
					text = stringBuilder.ToString();
				}
			}
			if (text.Contains("\t"))
			{
				string text2 = StringUtil.Replace(text, "\t", "¥t");
				string comparisonSource = text2;
string text3 = _translateText(comparisonSource);
if (comparisonSource != text3)
				{
					return StringUtil.Replace(text3, "¥t", "\t");
				}
				string text4 = StringUtil.Replace(text2, "\n", "<br>");
				text3 = _translateText(text4);
				if (text4 != text3)
				{
					return StringUtil.Replace(text3, "¥t", "\t");
				}
				if (tabCheck)
				{
					string[] array = StringUtil.Split(text, "\t");
					text = "";
					for (int j = 0; j < array.Length; j++)
					{
						if (j != 0)
						{
							text += "\t";
						}
						text += _translateText(array[j]);
					}
					return text;
				}
				return text;
			}
			{
string result = text;
			string text5 = _translateText(text);
			if (text5 != text)
			{
				return text5;
			}
			text = StringUtil.Replace(text, "\r\n", "<br>");
			text = StringUtil.Replace(text, "\n", "<br>");
			text5 = _translateText(text);
			if (text5 != text)
			{
				return text5;
			}
			return result;

}
}

		private static string _translateText(string text)
		{
			string text2 = text;
			string text3 = null;
			if (text2.StartsWith("<po="))
			{
				int num = text2.IndexOf('>');
				if (num != -1)
				{
					text3 = text2.Substring(0, num + 1);
					text2 = text2.Substring(num + 1);
				}
			}
			int num2 = text2.Length();
			if (textTable_ != null && num2 < textTable_.Length && textTable_[num2] != null)
			{
				for (int i = 0; i < textTable_[num2].Length; i++)
				{
					if (text2.Equals(textTable_[num2][i][0]))
					{
						if (text3 != null)
						{
							return text3 + textTable_[num2][i][1];
						}
						return textTable_[num2][i][1];
					}
				}
			}
			int num3 = Get();
			if (num3 != 0)
			{
				try
				{
					if (builtInTextTable_ == null)
					{
						for (int j = 0; j < BUILTIN_TRANSLATE_TABLE.Length; j++)
						{
							for (int k = 0; k < BUILTIN_TRANSLATE_TABLE[j].Length; k++)
							{
								BUILTIN_TRANSLATE_TABLE[j][k] = StringUtil.Replace(BUILTIN_TRANSLATE_TABLE[j][k], "\n", "<br>");
							}
						}
						builtInTextTable_ = MakeTextTable(BUILTIN_TRANSLATE_TABLE);
					}
					if (builtInTextTable_ != null && num2 < builtInTextTable_.Length && builtInTextTable_[num2] != null)
					{
						for (int l = 0; l < builtInTextTable_[num2].Length; l++)
						{
							if (text2.Equals(builtInTextTable_[num2][l][0]))
							{
								if (num3 >= builtInTextTable_[num2][l].Length)
								{
									num3 = 1;
								}
								if (langPackRecords_ != null && builtInTextTable_[num2][l].Length >= 3)
								{
									num3 = 2;
								}
								return (text3 == null) ? builtInTextTable_[num2][l][num3] : (text3 + builtInTextTable_[num2][l][num3]);
							}
						}
					}
				}
				catch
				{
				}
			}
			text2 = TranslateSoftLabel(text2);
			if (text3 != null)
			{
				return text3 + text2;
			}
			return text2;
		}

		public static string GetFolder(int language)
		{
			return FOLDERS[language];
		}

		public static string GetFolder()
		{
			return FOLDERS[Get()];
		}

		public static bool SetLanguagePack(string file, byte[] data = null, bool save = true)
		{
			if (Config.DEBUG)
			{
				if (file == null)
				{
					Log.Info("SetLanguagePack[(null)]");
				}
				else
				{
					Log.Info("SetLanguagePack[" + file + "]");
				}
			}
			if (file != null)
			{
				priorityLanguage_ = -1;
				if (English())
				{
					if (file.Equals(KAIRO_JAPANESE))
					{
						priorityLanguage_ = 0;
						ClearSoftLabelTable();
						ClearTextTable();
					}
				}
				else if (Japanese())
				{
					if (file.Equals("") || file.Equals(KAIRO_JAPANESE))
					{
						ClearSoftLabelTable();
						ClearTextTable();
					}
					else
					{
						priorityLanguage_ = 1;
					}
				}
			}
			if (file != null && (file.Equals(KAIRO_JAPANESE) || file.Equals(KAIRO_ENGLISH)))
			{
				if (save)
				{
					PlayerPrefs.SetString("_language_pack", file);
					PlayerPrefs.Save();
				}
				langPackFile_ = file;
				string id = "UA-35228877-18";
				AnalyticsTracker.TrackPageView(true, id, file);
				if (save)
				{
					if (Config.PRINT)
					{
						Log.Info("DispatchPageView[SetLanguagePack    MultiType]");
					}
					AnalyticsTracker.DispatchPageView();
				}
				return true;
			}
			bool flag = file != null && file.Length > 0 && !Regex.IsMatch(file, "^[a-zA-Z0-9_\\-]+\\.csv$");
			if (Config.PRINT)
			{
				if (file == null)
				{
					Log.Info("SetLanguagePack[(null)]    custom(" + flag + ")");
				}
				else
				{
					Log.Info("SetLanguagePack[" + file + "]    custom(" + flag + ")");
				}
			}
			JarInflater jarInflater = null;
			try
			{
				if (file != null && file.Length > 0 && data == null)
				{
					if (!flag)
					{
						jarInflater = new JarInflater(Storage.Read(3, false, "language.dat"));
						data = jarInflater.GetData(file);
					}
					else
					{
						if (!Storage.Exists(1, "_custom_lang_pack.csv"))
						{
							return false;
						}
						data = Storage.Read(1, false, "_custom_lang_pack.csv");
					}
				}
				LoadLanguagePack(data);
				if (!flag)
				{
					string title = GetTitle(langPackCode_);
					if (title != null)
					{
						langPackTitle_ = title;
					}
					langPackAuthor_ = "Kairosoft";
				}
				if (save)
				{
					if (flag)
					{
						if (PlayerPrefs.GetString("_language_pack_location", "") != file)
						{
							Dialog dialog = new Dialog(3, "Message");
							dialog.SetText("Data downloaded.\nUpload language pack and creator's name to Kairosoft for sharing?\nNo personal data will be sent.");
							int num = dialog.Show();
							PlayerPrefs.SetInt("_language_pack_send", (num == 4) ? 1 : 0);
						}
						PlayerPrefs.SetString("_language_pack_location", file);
						Storage.Write(1, false, "_custom_lang_pack.csv", data);
					}
					PlayerPrefs.SetString("_language_pack", file);
					PlayerPrefs.Save();
				}
				bool num2 = flag && PlayerPrefs.GetInt("_language_pack_send", 0) == 1;
				string id2 = "UA-35228877-18";
				if (num2)
				{
					AnalyticsTracker.TrackPageView(true, id2, langPackCode_, langPackAuthor_, URLEncoder.Encode(file));
				}
				else
				{
					AnalyticsTracker.TrackPageView(true, id2, langPackCode_);
				}
				if (save)
				{
					if (Config.PRINT)
					{
						Log.Info("DispatchPageView[SetLanguagePack]");
					}
					AnalyticsTracker.DispatchPageView();
				}
				langPackFile_ = file;
			}
			catch (Exception ex)
			{
				if (Config.PRINT)
				{
					Log.Info(ex);
				}
				if (flag)
				{
					string text = ex.Message.Trim();
					if (text.Length <= 0)
					{
						text = "An error has occurred.";
					}
					Dialog dialog2 = new Dialog(0, "Error");
					dialog2.SetText(text);
					dialog2.Show();
				}
				return false;
			}
			finally
			{
				if (((object)(jarInflater) != null)) { (jarInflater).Close(); }
				jarInflater = null;
			}
			return true;
		}

		public static string GetCustomLanguagePackLocation()
		{
			return PlayerPrefs.GetString("_language_pack_location", null);
		}

		private static void LoadLanguagePack(byte[] data)
		{
			JarInflater jarInflater = null;
			LangPackRecord[] array = langPackRecords_;
			string[] array2 = langPackFolders_;
			string text = langPackAuthor_;
			string text2 = langPackCode_;
			string text3 = langPackTitle_;
			int num = langPackScale_;
			langPackCode_ = "en-US";
			langPackAuthor_ = "Kairosoft";
			langPackTitle_ = "English";
			langPackScale_ = -1;
			if (Config.LANGUAGE == 0)
			{
				langPackCode_ = "ja-JP";
				langPackTitle_ = "日本語";
			}
			else if (Config.LANGUAGE == -1 && Japanese())
			{
				langPackCode_ = "ja-JP";
				langPackTitle_ = "日本語";
			}
			try
			{
				Dictionary<string, List<LangPackRecord>> dictionary = new Dictionary<string, List<LangPackRecord>>();
				if (data == null)
				{
					langPackRecords_ = null;
					langPackFolders_ = null;
				}
				else
				{
					byte[] array3 = Storage.Read(3, false, "language_pack_template_en.txt");
					if (array3 == null)
					{
						throw new Exception();
					}
					string text4 = StringUtil.GetString(array3);
					langPackRecords_ = new LangPackRecord[0];
					langPackFolders_ = null;
					LangPackRecord langPackRecord = null;
					int num2 = 0;
					while (num2 < text4.Length)
					{
						int num3 = text4.IndexOf(',', num2);
						int num4 = text4.IndexOf(',', num3 + 1);
						int num5 = text4.IndexOf(',', num4 + 1);
						int num6 = text4.IndexOf('\n', num5 + 1);
						if (num6 == -1)
						{
							num6 = text4.Length;
						}
						string text5 = text4.Substring(num2, num3 - num2).Trim();
						string file = text4.Substring(num3 + 1, num4 - (num3 + 1)).ToLower();
						string text6 = text4.Substring(num4 + 1, num5 - (num4 + 1)).Trim();
						string text7 = text4.Substring(num5 + 1, num6 - (num5 + 1));
						if (text5.Length > 0)
						{
							langPackRecord = new LangPackRecord(int.Parse(text5), file, text7);
							foreach (string positionText in text6.Split('&'))
							{
								string[] array5 = positionText.Split('-');
								langPackRecord.AddPosition(int.Parse(array5[0]), int.Parse(array5[1]));
							}
							while (langPackRecord.Id >= langPackRecords_.Length)
							{
								LangPackRecord[] destinationArray = new LangPackRecord[langPackRecords_.Length + 512];
								Array.Copy(langPackRecords_, 0, destinationArray, 0, langPackRecords_.Length);
								langPackRecords_ = destinationArray;
							}
							langPackRecords_[langPackRecord.Id] = langPackRecord;
							List<LangPackRecord> value;
							if (!dictionary.TryGetValue(langPackRecord.File, out value))
							{
								dictionary[langPackRecord.File] = (value = new List<LangPackRecord>());
							}
							value.Add(langPackRecord);
						}
						else
						{
							if (langPackRecord != null)
							{
								langPackRecord.From.Add(text7);
							}
						}
						num2 = num6 + 1;
					}
					int num7 = -1;
					{
					string text8 = null;
					using (MemoryStream stream = new MemoryStream(data))
					{
						using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
						{
							text8 = streamReader.ReadToEnd();
						}
					}
					new List<string[]>();
					langPackRecord = null;
					string text9 = "";
					string text10 = "";
					num2 = 0;
					while (num2 < text8.Length)
					{
						int num8 = text8.IndexOf(',', num2);
						int num9 = text8.IndexOf('\n', num8 + 1);
						if (num9 == -1)
						{
							num9 = text8.Length;
						}
						int num10 = num9;
						if (text8[num9 - 1] == '\r')
						{
							num9--;
						}
						string text11 = text8.Substring(num2, num8 - num2);
						string text12 = text8.Substring(num8 + 1, num9 - (num8 + 1));
						if (text11.StartsWith("\"") && text11.EndsWith("\""))
						{
							text11 = text11.Substring(1, text11.Length - 2);
						}
						if (text12.StartsWith("\"") && text12.EndsWith("\""))
						{
							text12 = text12.Substring(1, text12.Length - 2);
						}
						if (text11.StartsWith("@"))
						{
							if (text11 == "@author")
							{
								langPackAuthor_ = text12;
							}
							if (text11 == "@language")
							{
								langPackCode_ = Regex.Replace(text12, "[^a-zA-Z\\-]", "");
							}
							if (text11 == "@title")
							{
								langPackTitle_ = text12;
							}
							if (text11 == "@scale")
							{
								langPackScale_ = 100;
								if (text12.EndsWith("%"))
								{
									text12 = text12.Substring(0, text12.Length - 1);
								}
								if (text12.Equals("auto", StringComparison.CurrentCultureIgnoreCase))
								{
									langPackScale_ = -1;
								}
								else
								{
									int result = 0;
									if (int.TryParse(text12, out result))
									{
										langPackScale_ = result;
									}
								}
							}
							if (text11 == "@appli")
							{
								Match match = Regex.Match(text12, "([0-9]+)");
								if (match.Success)
								{
									num7 = int.Parse(match.Groups[1].Value);
								}
							}
						}
						else if (text11.StartsWith("#"))
						{
							int result2 = 0;
							if (int.TryParse(text11.Substring(1), out result2))
							{
								langPackRecord = langPackRecords_[result2];
								if (langPackRecord != null)
								{
									Match match2 = Regex.Match(langPackRecord.From[0], "^<po=[^>]+>");
									text9 = ((!match2.Success) ? "" : match2.Value);
									match2 = Regex.Match(langPackRecord.From[0], "</po>$");
									text10 = ((!match2.Success) ? "" : match2.Value);
									langPackRecord.To.Add(text9 + text12 + text10);
								}
							}
						}
						else if (text11.Length <= 0)
						{
							if (langPackRecord != null)
							{
								langPackRecord.To.Add(text9 + text12 + text10);
							}
						}
						num2 = num10 + 1;
					}
					}
					if (num7 != Config.PID)
					{
						throw new Exception("The language packs for different applications.");
					}
					if (langPackCode_ != null && langPackCode_.Length > 0)
					{
						string[] array6 = langPackCode_.Split('-');
						if (array6.Length == 2)
						{
							langPackCode_ = array6[0].ToLower() + "-" + array6[1].ToUpper();
						}
					}
					if (langPackTitle_.Length == 0)
					{
						langPackTitle_ = langPackCode_;
					}
					langPackFolders_ = null;
					if (langPackCode_ != null)
					{
						string[] array7 = langPackCode_.Split('-');
						List<string> list2 = new List<string>();
						list2.Add(langPackCode_);
						if (langPackCode_ == "zh-SG")
						{
							list2.Add("zh-CN");
						}
						if (langPackCode_ == "zh-HK")
						{
							list2.Add("zh-TW");
						}
						if (langPackCode_ == "zh-MO")
						{
							list2.Add("zh-TW");
						}
						if (array7.Length >= 2)
						{
							list2.Add(array7[0]);
						}
						langPackFolders_ = list2.ToArray();
					}
					langPackLines_ = new Dictionary<string, List<RecordPosition>[]>();
					foreach (KeyValuePair<string, List<LangPackRecord>> item in dictionary)
					{
						int num11 = 0;
						foreach (LangPackRecord item2 in item.Value)
						{
							foreach (Position position in item2.Positions)
							{
								if (num11 < position.Line)
								{
									num11 = position.Line;
								}
							}
						}
						num11++;
						List<RecordPosition>[] array8 = new List<RecordPosition>[num11];
						foreach (LangPackRecord item3 in item.Value)
						{
							foreach (Position position2 in item3.Positions)
							{
								if (array8[position2.Line] == null)
								{
									array8[position2.Line] = new List<RecordPosition>();
								}
								array8[position2.Line].Add(new RecordPosition(position2.Line, position2.Segment, item3));
							}
						}
						List<RecordPosition>[] array9 = array8;
						foreach (List<RecordPosition> list3 in array9)
						{
							if (list3 == null)
							{
								continue;
							}
							for (int j = 0; j < list3.Count - 1; j++)
							{
								for (int num12 = list3.Count - 1; num12 > j; num12--)
								{
									if (list3[num12 - 1].Segment > list3[num12].Segment)
									{
										RecordPosition value2 = list3[num12];
										list3[num12] = list3[num12 - 1];
										list3[num12 - 1] = value2;
									}
								}
							}
						}
						langPackLines_[item.Key] = array8;
					}
					List<LangPackRecord> value3 = null;
					if (dictionary.TryGetValue("lib.language", out value3))
					{
						byte[] array10 = Storage.Read(3, false, "language_pack_template_ja.txt");
						if (array10 == null)
						{
							throw new Exception();
						}
						string text13 = StringUtil.GetString(array10);
						Dictionary<string, List<int>> dictionary2 = new Dictionary<string, List<int>>();
						num2 = 0;
						while (num2 < text13.Length)
						{
							int num13 = text13.IndexOf(',', num2);
							int num14 = text13.IndexOf(',', num13 + 1);
							int num15 = text13.IndexOf(',', num14 + 1);
							int num16 = text13.IndexOf('\n', num15 + 1);
							if (num16 == -1)
							{
								num16 = text13.Length;
							}
							string text14 = text13.Substring(num2, num13 - num2).Trim();
							string text15 = text13.Substring(num13 + 1, num14 - (num13 + 1)).ToLower();
							text13.Substring(num14 + 1, num15 - (num14 + 1)).Trim();
							string key = text13.Substring(num15 + 1, num16 - (num15 + 1));
							if (text15 == "lib.language" && text14.Length > 0)
							{
								List<int> value4;
								if (!dictionary2.TryGetValue(key, out value4))
								{
									dictionary2[key] = (value4 = new List<int>());
								}
								value4.Add(int.Parse(text14));
							}
							num2 = num16 + 1;
						}
						string[][][] array11 = new string[2][][] { BUILTIN_TRANSLATE_TABLE, BUILTIN_SOFTLABEL_TABLE };
						for (int k = 0; k < array11.Length; k++)
						{
							for (int l = 0; l < array11[k].Length; l++)
							{
								string text16 = array11[k][l][1];
								int num17 = -1;
								List<int> value5;
								if (dictionary2.TryGetValue(array11[k][l][0], out value5))
								{
									foreach (int item4 in value5)
									{
										for (int m = 0; m < value3.Count; m++)
										{
											if (value3[m].Id == item4)
											{
												num17 = m;
												break;
											}
										}
										if (num17 == -1)
										{
											break;
										}
									}
								}
								else
								{
									for (int num18 = value3.Count - 1; num18 >= 0; num18--)
									{
										LangPackRecord langPackRecord2 = value3[num18];
										if (langPackRecord2.From[0].Length == text16.Length && langPackRecord2.From[0] == text16 && langPackRecord2.To.Count > 0)
										{
											num17 = num18;
											break;
										}
									}
								}
								if (num17 != -1)
								{
									LangPackRecord langPackRecord3 = value3[num17];
									if (array11[k][l].Length <= 2)
									{
										string[] array12 = new string[3];
										Array.Copy(array11[k][l], 0, array12, 0, 2);
										array11[k][l] = array12;
									}
									if (langPackRecord3.To.Count <= 0)
									{
										array11[k][l][2] = "";
									}
									else
									{
										array11[k][l][2] = langPackRecord3.To[0];
									}
									if (langPackRecord3.Positions.Count > 0)
									{
										langPackRecord3.Positions.RemoveAt(0);
									}
									if (langPackRecord3.Positions.Count <= 0)
									{
										value3.RemoveAt(num17);
									}
								}
							}
						}
						builtInTextTable_ = MakeTextTable(BUILTIN_TRANSLATE_TABLE);
					}
				}
				string[] array13 = null;
				string[][] array14 = null;
				jarInflater = new JarInflater(Storage.Read(3, false, "xls.dat"));
				if (jarInflater.Exist("softkey.txt"))
				{
					array13 = StringUtil.LoadStringArray(jarInflater, "softkey.txt");
					if (array13 != null)
					{
						array14 = new string[array13.Length][];
						for (int n = 0; n < array14.Length; n++)
						{
							array14[n] = StringUtil.Split(array13[n], "\t", false, false, false);
						}
						SetSoftLabelTable(array14);
					}
				}
				if (jarInflater.Exist("text.txt"))
				{
					array13 = StringUtil.LoadStringArray(jarInflater, "text.txt");
					if (array13 != null)
					{
						array14 = new string[array13.Length][];
						for (int num19 = 0; num19 < array14.Length; num19++)
						{
							array13[num19] = StringUtil.Replace(array13[num19], "\\n", "\n");
							array14[num19] = StringUtil.Split(array13[num19], "\t", false, false, false);
						}
						SetTextTable(array14);
					}
				}
				jarInflater.Close();
				jarInflater = null;
				kairo.unity.ui.Font.SetScale(100);
				if (langPackRecords_ != null)
				{
				if (langPackScale_ == -1 && dictionary.ContainsKey("text"))
				{
					List<LangPackRecord> list5 = new List<LangPackRecord>();
					foreach (LangPackRecord item5 in dictionary["text"])
					{
						if (item5.From.Count <= 1 && item5.To.Count <= 1 && item5.To.Count != 0 && 50 <= item5.From[0].Length && item5.From[0].Length < 200)
						{
							list5.Add(item5);
						}
					}
					int num20 = 0;
					int num21 = 0;
					int num22 = list5.Count / 20;
					if (num22 <= 0)
					{
						num22 = 1;
					}
					langPackScale_ = 0;
					kairo.unity.ui.Font font = new kairo.unity.ui.Font();
					while (num20 < list5.Count)
					{
						LangPackRecord langPackRecord4 = list5[num20];
						langPackScale_ += font.StringWidth(langPackRecord4.From[0]) * 100 / font.StringWidth(langPackRecord4.To[0]);
						num20 += num22;
						num21++;
					}
					if (num21 <= 0)
					{
						langPackScale_ = 100;
					}
					else
					{
						langPackScale_ /= num21;
					}
					if (langPackScale_ > 100)
					{
						langPackScale_ = 100;
					}
				}
				if (langPackScale_ <= 0)
				{
					langPackScale_ = 100;
				}
				if (langPackScale_ != 100)
				{
					kairo.unity.ui.Font.SetScale(langPackScale_);
				}
				}
			}
			catch (Exception ex)
			{
				langPackRecords_ = array;
				langPackFolders_ = array2;
				langPackAuthor_ = text;
				langPackCode_ = text2;
				langPackTitle_ = text3;
				langPackScale_ = num;
				throw ex;
			}
			finally
			{
				if (jarInflater != null)
				{
					jarInflater.Close();
				}
				jarInflater = null;
			}
		}

		internal static string[] GetLanguagePackFolders()
		{
			return langPackFolders_;
		}

		public static string[][] GetSystemLanguagePackFiles()
		{
			return systemLangPackFiles_;
		}

		public static string GetLanguagePackFile()
		{
			return langPackFile_;
		}

		public static string GetLanguageCode()
		{
			if (Japanese())
			{
				return "ja-JP";
			}
			return langPackCode_;
		}

		internal static byte[] ConvertLanguagePack(string file, byte[] data)
		{
			if (langPackRecords_ == null || !file.EndsWith(".txt"))
			{
				return data;
			}
			file = Path.GetFileNameWithoutExtension(file).ToLower();
			List<RecordPosition>[] value = null;
			if (!langPackLines_.TryGetValue(file, out value))
			{
				return data;
			}
			LangPackFormat value2 = null;
			if (!langPackFormats_.TryGetValue(file, out value2))
			{
				return data;
			}
			string text = StringUtil.GetString(data);
			if (text == null)
			{
				return data;
			}
			string[] array = text.Split(value2.LineCode);
			text = null;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(value2.LineCode);
				}
				if (i >= value.Length || value[i] == null || value[i].Count <= 0)
				{
					stringBuilder.Append(array[i]);
					array[i] = null;
					continue;
				}
				List<string> list = new List<string>(array[i].Split(value2.SegCode));
				array[i] = null;
				for (int num = value[i].Count - 1; num >= 0; num--)
				{
					RecordPosition recordPosition = value[i][num];
					if (recordPosition.Record.To.Count > 0)
					{
						list.RemoveRange(recordPosition.Segment, recordPosition.Record.From.Count);
						list.InsertRange(recordPosition.Segment, recordPosition.Record.To);
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (j != 0)
					{
						stringBuilder.Append(value2.SegCode);
					}
					stringBuilder.Append(list[j]);
				}
			}
			return stringBuilder.ToString().GetBytes();
		}

		public static LanguagePackInfo GetLanguagePackInfo(byte[] data)
		{
			LanguagePackInfo languagePackInfo = new LanguagePackInfo();
			if (data == null)
			{
				return languagePackInfo;
			}
			using (MemoryStream stream = new MemoryStream(data))
			{
				using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					while (true)
					{
						string text = streamReader.ReadLine();
						if (text == null)
						{
							break;
						}
						int num = text.IndexOf(',');
						if (num == -1)
						{
							break;
						}
						string text2 = text.Substring(0, num);
						string text3 = text.Substring(num + 1);
						Match match = Regex.Match(text2, "^\\s*\"(.*)\"\\s*$");
						if (match.Success)
						{
							text2 = match.Groups[1].Value;
						}
						text2 = text2.Trim().ToLower();
						match = Regex.Match(text3, "^\\s*\"(.*)\"\\s*$");
						if (match.Success)
						{
							text3 = match.Groups[1].Value;
						}
						if (!text2.StartsWith("@"))
						{
							break;
						}
						switch (text2)
						{
						case "@title":
							languagePackInfo.Title = text3;
							break;
						case "@language":
							languagePackInfo.Code = Regex.Replace(text3, "[^a-zA-Z\\-]", "");
							break;
						case "@scale":
							languagePackInfo.Scale = text3;
							break;
						case "@appli":
							languagePackInfo.Appli = text3;
							break;
						case "@version":
							match = Regex.Match(text3, "([0-9.]+)");
							if (match.Success)
							{
								languagePackInfo.Version = match.Groups[1].Value;
							}
							break;
						case "@author":
							languagePackInfo.Author = text3;
							break;
						}
					}
				}
			}
			string title = GetTitle(languagePackInfo.Code);
			if (title != null)
			{
				languagePackInfo.Title = title;
			}
			languagePackInfo.Author = "Kairosoft";
			return languagePackInfo;
		}

		public static void MakeLangPackTemplate(bool japanese, int version, out string file, out string data)
		{
			string filename = (japanese ? "language_pack_template_ja.txt" : "language_pack_template_en.txt");
			string text = StringUtil.GetString(Storage.Read(3, false, filename));
			string text2 = (japanese ? "ja-JP" : "en-US");
			string text3 = (japanese ? "日本語" : "English");
			StringBuilder stringBuilder = new StringBuilder();
			string text4 = version / 100 + "." + StringUtil.FormatString(string.Concat(version % 100), 2, '0');
			string[][] array = new string[6][]
			{
				new string[2] { "@title", text3 },
				new string[2] { "@language", text2 },
				new string[2] { "@scale", "auto" },
				new string[2]
				{
					"@appli",
					Config.PID + " - Do not change"
				},
				new string[2]
				{
					"@version",
					text4 + " - Do not change"
				},
				new string[2] { "@author", "Your Name" }
			};
			foreach (string[] array2 in array)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("\n");
				}
				stringBuilder.Append("\"" + array2[0] + "\",\"" + array2[1] + "\"");
			}
			int num = 0;
			while (num < text.Length)
			{
				int num2 = text.IndexOf(',', num);
				int num3 = text.IndexOf(',', num2 + 1);
				int num4 = text.IndexOf(',', num3 + 1);
				int num5 = text.IndexOf('\n', num4 + 1);
				if (num5 == -1)
				{
					num5 = text.Length;
				}
				string text5 = text.Substring(num, num2 - num);
				text.Substring(num2 + 1, num3 - (num2 + 1));
				text.Substring(num3 + 1, num4 - (num3 + 1));
				string text6 = text.Substring(num4 + 1, num5 - (num4 + 1));
				while (true) {
int length = text6.Length;
text6 = Regex.Replace(text6, "^<po=[^>]+>|</po>$", "");
if (length == text6.Length) { break; }
}
				if (text6.Length > 0)
				{
					if (text5.Length <= 0)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append("\n");
						}
						stringBuilder.Append(",\"" + text6.Replace("\"", "\"\"") + "\"");
					}
					else
					{
						while (text5.Length < 5)
						{
							text5 = "0" + text5;
						}
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append("\n");
						}
						stringBuilder.Append("\"#" + text5 + "\",\"" + text6.Replace("\"", "\"\"") + "\"");
					}
				}
				num = num5 + 1;
			}
			data = stringBuilder.ToString();
			file = Config.OUTPUT_CSV_NAME + (japanese ? "_ja" : "");
			file = Regex.Replace(file, "[^a-zA-Z_]", "");
			file += ".csv";
			if (Config.PRINT)
			{
				Log.Info("OUTPUT_NAME    file[" + file + "]");
			}
		}

		private static string GetTitle(string code)
		{
			string result = null;
			if (code != null)
			{
				code = code.ToLower();
			}
			if (Regex.IsMatch(code, "^ja"))
			{
				result = "日本語";
			}
			else if (Regex.IsMatch(code, "^en(-us)?$"))
			{
				result = "English";
			}
			else if (Regex.IsMatch(code, "^zh(-cn|-sg)"))
			{
				result = "中文(简体)";
			}
			else if (Regex.IsMatch(code, "^zh(-tw|-hk|-mo)?"))
			{
				result = "中文(繁體)";
			}
			else if (Regex.IsMatch(code, "^ko"))
			{
				result = "한국어";
			}
			return result;
		}
	}
}
