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

### GameInitializer.cs（オプショナル）
- **GameInitializer.cs**: ゲーム開始時に必要なGameObjectを自動生成する

## 依存関係

### 重要なポイント

**すべてのスクリプトは`GameInitializer`に依存していません。**

- `GameInitializer.cs`は**オプショナル**な自動初期化機能を提供します
- `GameInitializer`が存在しなくても、他のスクリプトは手動設定で動作します
- `GameInitializer`は他のファイルから参照されていません（完全に独立）

### 各ファイルの独立性

1. **PylosGamePresenter.cs**
   - 単独で動作可能
   - 他のコンポーネントから自動検索される

2. **InputController.cs**
   - カメラとプレゼンターを自動検索
   - Inspectorで設定することも可能

3. **BoardView.cs**
   - プレゼンターを自動検索
   - ボールプレハブが未設定の場合は自動生成

4. **PhaseView.cs**
   - プレゼンターを自動検索
   - UI要素を名前で自動検索（PhaseText、PlayerText、MessageText、MessagePanel）

5. **GameInitializer.cs**
   - 完全に独立したファイル
   - 存在しなくても他のスクリプトは動作する
   - 存在する場合は、ゲーム開始時に自動的に必要なGameObjectを生成

## 使用方法

### オプション1: GameInitializerを使用（推奨）

1. `GameInitializer.cs`をプロジェクトに含める
2. ゲームを開始すると、必要なGameObjectが自動生成される
3. 手動設定は不要

### オプション2: 手動設定

1. `GameInitializer.cs`を削除または無効化
2. 以下のGameObjectを手動で作成：
   - GameManager（PylosGamePresenterをアタッチ）
   - InputController（InputControllerをアタッチ）
   - BoardView（BoardViewをアタッチ）
   - Canvas（PhaseViewをアタッチ）
   - UI要素（PhaseText、PlayerText、MessagePanel、MessageText）
3. Inspectorで参照を設定

詳細は`MANUAL_SETUP.md`を参照してください。

## 設計思想

- **独立性**: 各コンポーネントは可能な限り独立して動作する
- **柔軟性**: 自動初期化と手動設定の両方をサポート
- **自動検索**: 参照が未設定でも自動的に検索を試みる
- **オプショナル機能**: `GameInitializer`は便利機能として提供されるが、必須ではない
