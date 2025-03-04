using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using toshi.VLiveKit.Photography;

[ExecuteAlways]
public class FpsCounterUI : MonoBehaviour {
    // [SerializeField]
    // private string bottomText = "subtitle";
    // フレームの指定enum
    public enum FrameType {
        FPS24,
        FPS30,
        FPS60,
    }
    [SerializeField]
    private FrameType frameType = FrameType.FPS24;


    // 変数
    int frameCounter;
    float lastUpdateTime;
    float currentFps;
    [SerializeField]
    private float displayWidth = 500;
    [SerializeField]
    private float displayHeight = 500;
    [SerializeField]
    private int fontSize = 48;
    [SerializeField]
    private Color fontColor = Color.white;
    [SerializeField]
    private float updateInterval = 0.5f;
    [SerializeField]
    private bool isShowFps = true;
    [SerializeField]
    private float displayX = 0; // 表示位置X
    [SerializeField]
    private float displayY = 0; // 表示位置Y

    // 赤い丸のパラメータ
    [SerializeField]
    private float circleSize = 20.0f; // 赤い丸のサイズ
    [SerializeField]
    private float circleOffsetX = 10.0f; // 右からのオフセット
    [SerializeField]
    private float circleOffsetY = 10.0f; // 上からのオフセット

    // モードテキスト
    [SerializeField]
    private string playModeText = "PLAY";
    [SerializeField]
    private string editorModeText = "EDITOR";

    [SerializeField]
    private bool isShowElapsedTime = false; // 経過時間を表示するかどうかのスイッチ

    private Texture2D circleTexture;

    [SerializeField]
    private int ltcFontSize = 48; // LTCのフォントサイズ
    [SerializeField]
    private Color ltcFontColor = Color.white; // LTCのフォントカラー
    [SerializeField]
    private float ltcDisplayX = 0; // LTCの表示位置X
    [SerializeField]
    private float ltcDisplayY = 0; // LTCの表示位置Y
    [SerializeField]
    private Color editorLtcFontColor = Color.gray; // エディタモード時のLTCフォントカラー

    [SerializeField]
    private PlayableDirector timelineDirector; // Timelineを制御するためのPlayableDirector
    [SerializeField]
    private bool useTimeline = false; // Timelineを使用するかどうかのスイッチ

    [SerializeField]
    private Text fpsText; // FPSを表示するためのTextコンポーネント

    [SerializeField]
    private float fpsDisplayX = 0; // FPS表示位置X
    [SerializeField]
    private float fpsDisplayY = 0; // FPS表示位置Y

    [SerializeField]
    private ColorPalette colorPalette; // スクリプタブルオブジェクトを参照

    // 初期化処理
    void Start()
    {
        // 変数の初期化
        frameCounter = 0;
        lastUpdateTime = 0.0f;

        // 元の赤い丸のテクスチャを作成
        circleTexture = CreateCircleTexture((int)circleSize, Color.white);

        // カラーパレットの各色の円形テクスチャを作成
        InitializeCircleTextures(); // スタート時に必ず初期化
    }

    // エディタでの変更を反映
    void OnValidate()
    {
        InitializeCircleTextures(); // エディタでの変更を常に反映
    }

    private void InitializeCircleTextures()
    {
        // カラーパレットの各色の円形テクスチャを作成
        circleTextures.Clear(); // リストをクリア
        if (colorPalette != null)
        {
            foreach (var color in colorPalette.colors)
            {
                circleTextures.Add(CreateCircleTexture((int)circleSize, color));
            }
        }
    }

    // 更新処理
    void Update()
    {
        if (isShowFps && Application.isPlaying){
            UpdateFps();
            UpdateFpsDisplay(); // FPS表示を更新
        }
        InitializeCircleTextures(); // 更新時に必ず初期化
    }

    void UpdateFpsDisplay()
    {
        if (fpsText != null)
        {
            fpsText.text = "FPS: " + currentFps.ToString("F2");
        }
    }

    void UpdateFps()
    {
        // フレームカウンタをインクリメント
        frameCounter++;
        // 経過時間を計算
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastUpdateTime + updateInterval)
        {
            // FPSを計算
            currentFps = frameCounter / (timeNow - lastUpdateTime);
            // フレームカウンタをリセット
            frameCounter = 0;
            // 経過時間を更新
            lastUpdateTime = timeNow;
        }
    }

    // 表示処理
    private void OnGUI()
    {
        // 描画順序を最前面に設定
        GUI.depth = 0;

        // 表示サイズを変更可能にする
        GUIStyle displayStyle = new GUIStyle();
        displayStyle.fontSize = fontSize; // 文字サイズを48に設定
        displayStyle.normal.textColor = fontColor; // フォントカラーを白に設定

        if (isShowFps)
        {
            // FPSを表示
            Rect fpsRect = new Rect(fpsDisplayX, fpsDisplayY, displayWidth, displayHeight);
            GUILayout.BeginArea(fpsRect);
            GUILayout.Label("FPS: " + currentFps.ToString("F2"), displayStyle, GUILayout.Width(displayWidth), GUILayout.Height(displayHeight));
            GUILayout.EndArea();
        }

        if (useTimeline && timelineDirector != null)
        {
            // Timelineを使用してLTCを制御
            double timelineTime = timelineDirector.time;
            int hours = (int)(timelineTime / 3600);
            int minutes = (int)((timelineTime % 3600) / 60);
            int seconds = (int)(timelineTime % 60);
            int frames = (int)((timelineTime * 24) % 24); // 1秒に24フレーム進む
            switch (frameType)
            {
                case FrameType.FPS24:
                    frames = (int)((timelineTime * 24) % 24); // 1秒に24フレーム進む
                    break;
                case FrameType.FPS30:
                    frames = (int)((timelineTime * 30) % 30); // 1秒に30フレーム進む
                    break;
                case FrameType.FPS60:
                    frames = (int)((timelineTime * 60) % 60); // 1秒に60フレーム進む
                    break;
            }
            string timeString = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", hours, minutes, seconds, frames);

            GUIStyle ltcStyle = new GUIStyle();
            ltcStyle.fontSize = ltcFontSize;

            // Timelineが停止している場合はエディタモードの色を使用
            if (timelineDirector.state == PlayState.Paused)
            {
                ltcStyle.normal.textColor = editorLtcFontColor;
            }
            else
            {
                ltcStyle.normal.textColor = ltcFontColor;
            }

            Rect ltcRect = new Rect(ltcDisplayX, ltcDisplayY, displayWidth, displayHeight);
            GUILayout.BeginArea(ltcRect);
            GUILayout.Label(timeString, ltcStyle, GUILayout.Width(displayWidth), GUILayout.Height(displayHeight));
            GUILayout.EndArea();
        }

        // 元の赤い丸を表示
        float x = Screen.width - circleSize - circleOffsetX; // 右上の位置
        float y = circleOffsetY; // 上からの位置
        Color originalColor = GUI.color;

        if (Application.isPlaying)
        {
            // プレイモード時は赤い丸を点滅させる
            GUI.color = new Color(1.0f, 0.0f, 0.0f, Mathf.PingPong(Time.time, 1.0f)); // 点滅効果
        }
        else
        {
            // 非プレイモード時は緑色の丸を表示
            GUI.color = Color.green;
        }

        GUI.DrawTexture(new Rect(x, y, circleSize, circleSize), circleTexture);
        GUI.color = originalColor;

        // カラーパレットの丸を画面下に表示
        float paletteX = paletteOffsetX; // カラーパレットの開始位置X
        float paletteY = Screen.height - circleSize - paletteOffsetY; // カラーパレットの開始位置Y
        for (int i = 0; i < circleTextures.Count; i++)
        {
            GUI.color = Color.white; // テクスチャの色をそのまま使用
            GUI.DrawTexture(new Rect(paletteX + i * (circleSize + circleSpacing), paletteY, circleSize, circleSize), circleTextures[i]);
        }

        GUI.color = originalColor;

        // モードテキストを表示
        GUIStyle modeTextStyle = new GUIStyle();
        modeTextStyle.fontSize = fontSize;
        modeTextStyle.normal.textColor = fontColor;

        string modeText = Application.isPlaying ? playModeText : editorModeText;
        GUI.Label(new Rect(x + circleSize + 5, y, 100, circleSize), modeText, modeTextStyle);

        // 画面下にテキストを表示
        GUIStyle bottomTextStyle = new GUIStyle();
        bottomTextStyle.fontSize = fontSize;
        bottomTextStyle.normal.textColor = bottomTextColor; // テキストの色を設定
        bottomTextStyle.alignment = TextAnchor.LowerCenter; // テキストを中央に配置

        Rect bottomRect = new Rect(bottomTextX, Screen.height - fontSize - 10 + bottomTextY, Screen.width, fontSize);
        GUI.Label(bottomRect, bottomText, bottomTextStyle);
    }

    // 円形テクスチャを作成するメソッド
    private Texture2D CreateCircleTexture(int diameter, Color color)
    {
        Texture2D texture = new Texture2D(diameter, diameter, TextureFormat.ARGB32, false);
        Color[] colors = new Color[diameter * diameter];
        float radius = diameter / 2f;
        Vector2 center = new Vector2(radius, radius);

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                Vector2 pos = new Vector2(x, y);
                if (Vector2.Distance(pos, center) <= radius)
                {
                    colors[y * diameter + x] = color;
                }
                else
                {
                    colors[y * diameter + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    [SerializeField]
    private string bottomText = "ここに表示するテキスト"; // 画面下に表示するテキスト
    [SerializeField]
    private float bottomTextX = 0; // 画面下テキストのX位置
    [SerializeField]
    private float bottomTextY = 0; // 画面下テキストのY位置
    [SerializeField]
    private Color bottomTextColor = Color.white; // 画面下テキストの色

    [SerializeField]
    private float circleSpacing = 5.0f; // カラーパレットの丸の間隔
    [SerializeField]
    private float paletteOffsetX = 10.0f; // カラーパレットのXオフセット
    [SerializeField]
    private float paletteOffsetY = 10.0f; // カラーパレットのYオフセット

    private List<Texture2D> circleTextures = new List<Texture2D>(); // カラーパレットの丸のテクスチャリスト
}
