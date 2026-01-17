# Unity セットアップガイド（初心者向け）

このガイドでは、Unityエディタでピロスゲームを手動設定する方法を、初心者にも分かりやすく説明します。

---

## 📚 目次

1. [Unityの基本概念](#unityの基本概念)
2. [必要な準備](#必要な準備)
3. [手順1: GameManagerの作成](#手順1-gamemanagerの作成)
4. [手順2: InputControllerの作成](#手順2-inputcontrollerの作成)
5. [手順3: BoardViewの作成](#手順3-boardviewの作成)
6. [手順4: ボールプレハブの作成](#手順4-ボールプレハブの作成)
7. [手順5: UI（PhaseView）の作成](#手順5-ui-phaseviewの作成)
8. [手順6: カメラの設定](#手順6-カメラの設定)
9. [手順7: 盤面のCollider設定](#手順7-盤面のcollider設定)
10. [確認とテスト](#確認とテスト)
11. [トラブルシューティング](#トラブルシューティング)

---

## Unityの基本概念

### GameObject（ゲームオブジェクト）
- Unityのシーンに配置されるすべての要素
- 例：カメラ、ライト、プレイヤー、UI要素など
- 空のGameObjectも作成できます

### Component（コンポーネント）
- GameObjectにアタッチされる機能
- 例：Transform（位置・回転・スケール）、Camera（カメラ機能）、Text（テキスト表示）など
- 1つのGameObjectに複数のコンポーネントをアタッチできます

### Inspector（インスペクター）
- 選択したGameObjectの詳細を表示・編集するウィンドウ
- コンポーネントの追加・削除、設定の変更ができます

### Hierarchy（ヒエラルキー）
- シーン内のすべてのGameObjectを表示するウィンドウ
- 親子関係も表示されます

### Project（プロジェクト）
- プロジェクト内のファイル（スクリプト、画像、プレハブなど）を表示するウィンドウ

### Prefab（プレハブ）
- 再利用可能なGameObjectのテンプレート
- 一度作成すれば、何度でも使えます

---

## 必要な準備

1. Unityエディタが開いている
2. `SampleScene.unity`が開いている
3. プロジェクトに必要なスクリプトがインポートされている

---

## 手順1: GameManagerの作成

#### 1-1. 空のGameObjectを作成
1. **Hierarchyウィンドウ**（通常は画面左側）を見る
2. Hierarchyウィンドウ内で**右クリック**
3. メニューから**`Create Empty`**を選択
4. 新しいGameObjectが作成されます（名前は「GameObject」）

#### 1-2. 名前を変更
1. 作成したGameObjectを**クリック**して選択
2. 名前の部分を**クリック**して編集可能にする
3. 名前を**`GameManager`**に変更
4. **Enterキー**で確定

#### 1-3. コンポーネントをアタッチ
1. `GameManager`が選択されていることを確認（Hierarchyでクリック）
2. **Inspectorウィンドウ**（通常は画面右側）を見る
3. Inspectorウィンドウの下部にある**`Add Component`**ボタンをクリック
4. 検索ボックスに**`PylosGamePresenter`**と入力
5. 候補から**`Pylos Game Presenter`**を選択

### 確認
- Inspectorウィンドウに**`Pylos Game Presenter`**コンポーネントが表示されていることを確認

---

## 手順2: InputControllerの作成

#### 2-1. GameObjectを作成
1. Hierarchyで**右クリック** → **`Create Empty`**
2. 名前を**`InputController`**に変更

#### 2-2. コンポーネントをアタッチ
1. `InputController`を選択
2. Inspectorで**`Add Component`** → **`Input Controller`**を追加

#### 2-3. 参照を設定
1. Inspectorの**`Input Controller`**コンポーネントを見る
2. **`Main Camera`**フィールドの右側にある**円形のアイコン**をクリック
3. または、Hierarchyの**`Main Camera`**を**ドラッグ&ドロップ**して設定
4. **`Presenter`**フィールドに、Hierarchyの**`GameManager`**を**ドラッグ&ドロップ**

### 確認
- `Main Camera`と`Presenter`が設定されていることを確認
- 設定されていない場合でも、自動検索されるので動作します（ただし、明示的に設定することを推奨）

---

## 手順3: BoardViewの作成

#### 3-1. GameObjectを作成
1. Hierarchyで**右クリック** → **`Create Empty`**
2. 名前を**`BoardView`**に変更

#### 3-2. コンポーネントをアタッチ
1. `BoardView`を選択
2. Inspectorで**`Add Component`** → **`Board View`**を追加

#### 3-3. プレゼンター参照を設定（オプション）
1. Inspectorの**`Board View`**コンポーネントで
2. **`Presenter`**フィールドに、Hierarchyの**`GameManager`**を**ドラッグ&ドロップ**
3. 設定しなくても自動検索されます

### 確認
- `Board View`コンポーネントがアタッチされていることを確認

---

## 手順4: ボールプレハブの作成

#### 4-1. 白いボールの作成

##### 4-1-1. Sphereを作成
1. Hierarchyで**右クリック** → **`3D Object`** → **`Sphere`**
2. 名前を**`WhiteBall`**に変更

##### 4-1-2. サイズを調整
1. `WhiteBall`を選択
2. Inspectorの**`Transform`**コンポーネントを見る
3. **`Scale`**を`(0.8, 0.8, 0.8)`に変更
   - X: 0.8
   - Y: 0.8
   - Z: 0.8

##### 4-1-3. Material（マテリアル）を作成
1. **Projectウィンドウ**（通常は画面下部）を見る
2. Projectウィンドウ内で**右クリック**
3. **`Create`** → **`Material`**
4. 名前を**`WhiteBallMaterial`**に変更

##### 4-1-4. Materialの色を設定
1. `WhiteBallMaterial`を選択
2. Inspectorで**`Albedo`**（色の部分）をクリック
3. カラーピッカーで**白（#FFFFFF）**を選択
4. または、RGBを`(255, 255, 255)`に設定

##### 4-1-5. Materialをボールに適用
1. Projectウィンドウの**`WhiteBallMaterial`**を
2. Hierarchyの**`WhiteBall`**に**ドラッグ&ドロップ**

##### 4-1-6. Colliderを削除（オプション）
1. `WhiteBall`を選択
2. Inspectorで**`Sphere Collider`**コンポーネントを見つける
3. **`Sphere Collider`**の右上にある**⋮（三点メニュー）**をクリック
4. **`Remove Component`**を選択
5. または、右クリック → **`Remove Component`**

##### 4-1-7. プレハブ化
1. Projectウィンドウで、**`Assets`**フォルダを開く（または適切なフォルダを作成）
2. Hierarchyの**`WhiteBall`**を
3. Projectウィンドウの**`Assets`**フォルダに**ドラッグ&ドロップ**
4. プレハブが作成されます（青いアイコンになる）
5. Hierarchyの**`WhiteBall`**を**削除**（右クリック → **`Delete`**、または**Deleteキー**）

#### 4-2. 黒いボールの作成
1. 4-1と同じ手順で**`BlackBall`**を作成
2. Materialの色を**黒（#000000）**に設定
3. プレハブ化

#### 4-3. プレハブをBoardViewに設定
1. Hierarchyで**`BoardView`**を選択
2. Inspectorの**`Board View`**コンポーネントを見る
3. **`White Ball Prefab`**フィールドに、Projectウィンドウの**`WhiteBall`**プレハブを**ドラッグ&ドロップ**
4. **`Black Ball Prefab`**フィールドに、Projectウィンドウの**`BlackBall`**プレハブを**ドラッグ&ドロップ**

### 確認
- `White Ball Prefab`と`Black Ball Prefab`が設定されていることを確認
- プレハブが設定されていない場合でも、自動生成されるので動作します

---

## 手順5: UI（PhaseView）の作成

#### 5-1. Canvasの作成
1. Hierarchyで**右クリック** → **`UI`** → **`Canvas`**
2. Canvasが作成されます（自動的に`EventSystem`も作成されます）

#### 5-2. PhaseTextの作成

##### 5-2-1. Textを作成
1. Hierarchyで**`Canvas`**を選択
2. **右クリック** → **`UI`** → **`Text`** → **`Text`**
3. 名前を**`PhaseText`**に変更

##### 5-2-2. Textの設定
1. `PhaseText`を選択
2. Inspectorの**`Text`**コンポーネントで以下を設定：
   - **`Text`**: "設置フェーズ"
   - **`Font Size`**: 24
   - **`Alignment`**: 左上（左上のアイコンをクリック）

##### 5-2-3. 位置を調整
1. Inspectorの**`Rect Transform`**コンポーネントを見る
2. **`Anchor Presets`**をクリック（左上のボタン）
3. **`Alt + Shift`キーを押しながら**、左上のアイコンをクリック
4. これで、左上を基準に配置されます
5. **`Pos X`**: 10
6. **`Pos Y`**: -10
7. **`Width`**: 200
8. **`Height`**: 30

#### 5-3. PlayerTextの作成
1. `Canvas`を選択
2. **右クリック** → **`UI`** → **`Text`** → **`Text`**
3. 名前を**`PlayerText`**に変更
4. Inspectorで設定：
   - **`Text`**: "現在のプレイヤー: 白"
   - **`Font Size`**: 24
   - **`Alignment`**: 左上
   - **`Rect Transform`**:
     - **`Anchor Presets`**: 左上（Alt+Shift+クリック）
     - **`Pos X`**: 10
     - **`Pos Y`**: -50
     - **`Width`**: 200
     - **`Height`**: 30

#### 5-4. MessagePanelの作成

##### 5-4-1. Panelを作成
1. `Canvas`を選択
2. **右クリック** → **`UI`** → **`Panel`**
3. 名前を**`MessagePanel`**に変更

##### 5-4-2. Panelの設定
1. `MessagePanel`を選択
2. Inspectorの**`Image`**コンポーネントで：
   - **`Color`**をクリック
   - **`Alpha`**（透明度）を**204**に設定（約80%の不透明度）
   - または、RGBを`(0, 0, 0)`、Alphaを`204`に設定

##### 5-4-3. Panelのサイズと位置
1. Inspectorの**`Rect Transform`**で：
   - **`Anchor Presets`**: 中央（Alt+Shift+クリック）
   - **`Width`**: 400
   - **`Height`**: 200

##### 5-4-4. Panelを非アクティブ化
1. Inspectorの上部、**`MessagePanel`**の左側にある**チェックボックス**を**外す**
2. これで、初期状態では非表示になります

#### 5-5. MessageTextの作成
1. **`MessagePanel`**を選択（非アクティブでも選択できます）
2. **右クリック** → **`UI`** → **`Text`** → **`Text`**
3. 名前を**`MessageText`**に変更
4. Inspectorで設定：
   - **`Text`**: "ゲーム終了！"
   - **`Font Size`**: 32
   - **`Alignment`**: 中央
   - **`Rect Transform`**:
     - **`Anchor Presets`**: 全画面（Alt+Shift+クリック）
     - **`Left`**, **`Right`**, **`Top`**, **`Bottom`**: すべて**0**

#### 5-6. PhaseViewコンポーネントの追加
1. **`Canvas`**を選択
2. Inspectorで**`Add Component`** → **`Phase View`**を追加

#### 5-7. PhaseViewの参照を設定
1. **`Canvas`**を選択
2. Inspectorの**`Phase View`**コンポーネントで：
   - **`Phase Text`**に、Hierarchyの**`PhaseText`**を**ドラッグ&ドロップ**
   - **`Player Text`**に、Hierarchyの**`PlayerText`**を**ドラッグ&ドロップ**
   - **`Message Text`**に、Hierarchyの**`MessageText`**を**ドラッグ&ドロップ**
   - **`Message Panel`**に、Hierarchyの**`MessagePanel`**を**ドラッグ&ドロップ**
   - **`Presenter`**に、Hierarchyの**`GameManager`**を**ドラッグ&ドロップ**

### 確認
- すべての参照が設定されていることを確認
- 設定されていない場合でも、自動検索されるので動作します（ただし、明示的に設定することを推奨）

---

## 手順6: カメラの設定

#### 6-1. カメラを選択
1. Hierarchyで**`Main Camera`**を選択

#### 6-2. 位置を調整
1. Inspectorの**`Transform`**コンポーネントで：
   - **`Position`**:
     - **X**: 0
     - **Y**: 5
     - **Z**: -5

#### 6-3. 角度を調整
1. Inspectorの**`Transform`**コンポーネントで：
   - **`Rotation`**:
     - **X**: 45
     - **Y**: 0
     - **Z**: 0

### 確認
1. **Sceneビュー**（通常は画面中央上部）を見る
2. 盤面が見える位置にカメラがあることを確認
3. 必要に応じて、Sceneビューでカメラをドラッグして調整

---

## 手順7: 盤面のCollider設定

#### 7-1. Planeを作成
1. Hierarchyで**右クリック** → **`3D Object`** → **`Plane`**
2. 名前を**`Board`**に変更

#### 7-2. 位置とサイズを調整
1. `Board`を選択
2. Inspectorの**`Transform`**で：
   - **`Position`**: `(0, 0, 0)`
   - **`Scale`**: `(2, 1, 2)`
     - 盤面のサイズに合わせて調整してください

### 確認
- `Board`に**`Mesh Collider`**がアタッチされていることを確認（自動的にアタッチされます）

---

## 確認とテスト

### チェックリスト
- [ ] `GameManager`に`PylosGamePresenter`がアタッチされている
- [ ] `InputController`に`InputController`がアタッチされ、`Main Camera`と`Presenter`が設定されている
- [ ] `BoardView`に`BoardView`がアタッチされている
- [ ] `Canvas`に`PhaseView`がアタッチされ、すべてのUI要素が設定されている
- [ ] `PhaseText`、`PlayerText`、`MessagePanel`、`MessageText`が作成されている
- [ ] 白と黒のボールプレハブが作成され、`BoardView`に設定されている
- [ ] カメラが適切な位置に配置されている
- [ ] 盤面用の`Board`（Plane）が作成されている

### テスト方法
1. Unityエディタの上部にある**▶（Play）ボタン**をクリック
2. Gameビューで以下を確認：
   - UIが表示されている（PhaseText、PlayerText）
   - カメラが盤面を見ている
3. 盤面をクリックして、ボールが配置されるか確認

---

## トラブルシューティング

### ボールが表示されない
- **原因**: ボールプレハブが設定されていない
- **解決方法**: 
  1. `BoardView`を選択
  2. Inspectorで`White Ball Prefab`と`Black Ball Prefab`が設定されているか確認
  3. 設定されていない場合は、手順4を参照してプレハブを作成

### UIが表示されない
- **原因**: Canvasが非アクティブ、またはUI要素が設定されていない
- **解決方法**:
  1. Hierarchyで`Canvas`がアクティブ（チェックが入っている）か確認
  2. `PhaseView`の参照が正しく設定されているか確認

### クリックが反応しない
- **原因**: Colliderが設定されていない、またはカメラが設定されていない
- **解決方法**:
  1. `Board`（Plane）に`Mesh Collider`がアタッチされているか確認
  2. `InputController`の`Main Camera`が設定されているか確認

### エラーが出る
- **原因**: 参照が正しく設定されていない
- **解決方法**:
  1. **Consoleウィンドウ**（Window → General → Console）でエラーメッセージを確認
  2. エラーメッセージに従って、参照を正しく設定

### カメラが見えない位置にある
- **原因**: カメラの位置や角度が適切でない
- **解決方法**:
  1. Sceneビューでカメラの位置を確認
  2. 手順6を参照して、カメラの位置と角度を調整

---

## まとめ

このガイドに従って設定すれば、ピロスゲームが動作するはずです。各手順は独立しているので、必要に応じて個別に確認・修正できます。

問題が発生した場合は、**Consoleウィンドウ**でエラーメッセージを確認し、該当する手順を見直してください。

---

## 補足：Unityの基本操作

### ウィンドウの表示
- **Hierarchy**: Window → General → Hierarchy
- **Inspector**: Window → General → Inspector
- **Project**: Window → General → Project
- **Console**: Window → General → Console

### よく使うショートカット
- **Ctrl + S** (Mac: **Cmd + S**): シーンを保存
- **F**: 選択したオブジェクトにフォーカス
- **Delete**: 選択したオブジェクトを削除
- **Ctrl + Z** (Mac: **Cmd + Z**): 元に戻す

### ドラッグ&ドロップ
- ProjectウィンドウからHierarchyへ: オブジェクトを配置
- HierarchyからProjectへ: プレハブを作成
- HierarchyからInspectorへ: 参照を設定

---

お疲れ様でした！設定が完了したら、ゲームを楽しんでください！
