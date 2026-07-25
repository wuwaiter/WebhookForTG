# WebhookForTG

接收 Webhook 訊息並同步轉發到 **LINE Notify** 與 **Telegram Bot**。

## 技術規格

- .NET 8 Web API
- `dotenv.net` 讀取 `.env`
- 支援 Docker / Docker Compose

---

## 快速開始

### 1. 取得必要 Token

#### LINE Notify
1. 前往 [LINE Notify 官網](https://notify-bot.line.me/)
2. 登入後點選「個人頁面」→「發行存取權杖」
3. 輸入服務名稱，選擇要發送的聊天群組，複製產生的 Token

#### Telegram Bot
1. 在 Telegram 搜尋 `@BotFather`，輸入 `/newbot` 建立機器人
2. 複製取得的 **Bot Token**（格式：`123456789:ABCDefgh...`）
3. 取得 **Chat ID**：
   - 將 Bot 加入目標群組（或直接與 Bot 私訊）
   - 瀏覽 `https://api.telegram.org/bot<YOUR_TOKEN>/getUpdates`
   - 在回傳 JSON 中找到 `chat.id`

### 2. 設定 `.env`

```bash
cp .env.example .env
```

編輯 `.env`：

```
Telegram__BotToken=你的 Telegram Bot Token
Telegram__ChatId=你的 Chat ID
```

> `.env` 已在 `.gitignore`，不會被 commit。

### 3. 執行（本機）

```bash
dotnet run
```

預設約在 `http://localhost:5261` 啟動。

### 4. 執行（Docker）

```bash
docker compose up --build -d
```

服務會在 `http://localhost:8080` 啟動。

常用指令：

```bash
docker compose logs -f    # 看 log
docker compose down       # 停止並移除容器
```

---

## API 端點

### POST `/api/webhook`

JSON Body 發送：

```json
{
  "message": "你好，這是一則測試訊息",
  "title": "系統通知",
  "source": "MyApp"
}
```

| 欄位 | 必填 | 說明 |
|------|------|------|
| `message` | ✅ | 訊息內容 |
| `title` | ❌ | 標題，若有則顯示為 `[標題]\n訊息` |
| `source` | ❌ | 來源標識（僅記錄 log） |

---

### POST `/api/webhook/simple`

Query String 發送（適合簡易串接）：

```
POST /api/webhook/simple?message=測試訊息&title=通知
```

---

## 驗證機制

若 `appsettings.json` 中有設定 `Webhook:SecretKey`，請求需帶上 Header：

```
X-Webhook-Secret: 你的金鑰
```

若 `SecretKey` 為空，則不需要驗證。

---

## 回傳格式

```json
{
  "lineSuccess": true,
  "telegramSuccess": true,
  "lineError": null,
  "telegramError": null
}
```

- 兩者皆成功 → HTTP `200`
- 部分失敗 → HTTP `207 Multi-Status`

---

## cURL 範例

```bash
# JSON 格式
curl -X POST http://localhost:8080/api/webhook \
  -H "Content-Type: application/json" \
  -d '{"message":"伺服器 CPU 超過 90%","title":"警告","source":"monitoring"}'

# 簡易格式
curl -X POST "http://localhost:8080/api/webhook/simple?message=Hello&title=Test"
```
