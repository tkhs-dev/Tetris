# 課題研究 テトリスを人工知能でプレイ
![.NET Core](https://github.com/tkhs-dev/Tetris/workflows/.NET%20Core/badge.svg)
[![GitHub version](https://badge.fury.io/gh/tkhs-dev%2FTetris.svg)](https://badge.fury.io/gh/tkhs-dev%2FTetris)
![GitHub issues](https://img.shields.io/github/issues/tkhs-dev/Tetris)
![Nuget](https://img.shields.io/nuget/v/log4net?label=log4net)
![Nuget](https://img.shields.io/nuget/v/geneticsharp?label=GeneticSharp)

## 概要
次の設置可能な盤面をすべて予想し、ニューラルネットワークでその盤面の評価値を算出し最も評価が高い物を次の行動として採用する。
ニューラルネットワークの各バイアスを遺伝的アルゴリズムで調整し、学習させる。

## 設計
* **TetrisCore**          
テトリスのコア部分
* ~~**TetrisPlayer**~~  
~~Windows .Formsを使用したシングルプレイ用のプレイヤー~~  
TetrisPlayerWPFに移行
* **TetrisPlayerWPF**  
WPFを使用したプレイヤー シングルプレイ AIプレイ リプレイ(バグあり)を実装済み
* **TetrisDXControl**  
Windows.FormsのDXを使ったテトリスコントロール TetrisPlayerWPFでも使用
* **TetrisAI**          
テトリスAIの基本部分 ニューラルネット
* **TetrisAI-Trainer**  
テトリスAIの学習部分 遺伝的アルゴリズム
* **TetrisTest**  
単体テストプロジェクト

機械学習の実装は時間がかかるのでライブラリを利用する。

## 学習
### ニューラルネットワーク
  #### 目的
  * フィールドから特徴量を抽出して評価値を算出する。
  #### 特徴量
  * フィールド上の穴の数
  * 穴のある列の数
  * 穴の上のブロックの数
  * 入り組んだスペースの数
  * 設置したオブジェクトの高さ
  * 消されたラインの数
  * 設置したオブジェクトから消えたブロックの数
  * X方向のブロックの変化の合計
  * Y方向のブロックの変化の合計
  #### 実装
  * KelpNetを使用
  * 入力層:9 中間層:5 出力層:1
  * 活性化関数: 恒等関数
### 遺伝的アルゴリズム
  #### 目的
  * ニューラルネットワークの各バイアスを適切に調整する。
  #### 実装
  * GeneticSharpを使用
  * 数試合の平均スコアを適応度とする
  * 遺伝子 [TetrisChromosomes](/TetrisAI-Trainer/Source/ga/TetrisChromosomes.cs)
  * 適応度 [TetrisFitness](/TetrisAI-Trainer/Source/ga/TetrisFitness.cs)
  * 交叉 [TetrisCrossover](/TetrisAI-Trainer/Source/ga/TetrisCrossover.cs)
  * 突然変異 均一
