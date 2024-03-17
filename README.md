# hsr_moe_quiz_opt

崩壊スターレイル 教育部の難問 の一部を数理最適化で解く

## 実行方法

各問題番号のディレクトリに移動して

```shell
$ dotnet run
```

## 補足: $a \land b \Rightarrow c$ の定式化について

$a$, $b$, $c$ を $0\text{-}1$ 決定変数として, $a \land b \Rightarrow c$ の形の制約条件をどのように表せばよいか. 
これは対偶を取ると導出することができる. 
対偶を取ると $\lnot c \Rightarrow \lnot a \lor \lnot b$ となり, これを制約式に展開すると

$$
1 - c \le (1 - a) + (1 - b).
$$

変形して

$$
a + b \le 1 + c
$$

を得る. 
ちなみに

- $a \land b \Rightarrow c$ は $a + b \le 1 + c$ (上記)
- $a \lor b \Rightarrow c$ は $a + b \le 2 c$
- $a \Rightarrow b \land c$ は $2 a \le b + c$
- $a \Rightarrow b \lor c$ は $a \le b + c$

と定式化できる. 

