# Frontend スクリプト構成

## 概要

このディレクトリには、Unityのフロントエンド（表示・入力）に関するスクリプトが含まれています。

## ファイル構成

### Controllers/
- **PylosGamePresenter.cs**: バックエンドとフロントエンドを繋ぐ橋渡し役
- **InputController.cs**: マウスクリックなどの入力を検知

### Views/
- **BoardView.cs**: 盤面の3D表示（ボールの配置・削除）
- **PhaseView.cs**: UI表示（フェーズ、プレイヤー情報、メッセージ）
- **BallView.cs**: ボール単体の制御（現在は未実装）

## 使用方法

### 手動設定

Unityエディタで以下のGameObjectを手動で作成し、設定してください：

1. **GameManager**（PylosGamePresenterをアタッチ）
2. **InputController**（InputControllerをアタッチ）
3. **BoardView**（BoardViewをアタッチ）
4. **Canvas**（PhaseViewをアタッチ）
5. **UI要素**（PhaseText、PlayerText、MessagePanel、MessageText）
6. **ボールプレハブ**（白と黒）

詳細な手順は、プロジェクトルートの**`SETUP_GUIDE.md`**を参照してください。

## 設計思想

- **独立性**: 各コンポーネントは可能な限り独立して動作する
- **自動検索**: 参照が未設定でも自動的に検索を試みる
- **柔軟性**: Inspectorで明示的に設定することも、自動検索に任せることも可能
