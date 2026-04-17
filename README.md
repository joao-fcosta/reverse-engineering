# <p align="center">Reverse Engineering</p>

<p align="center">
  <strong>Atividade Prática - Disciplina de Desing e Melhoria de Software</strong><br>
  Engenharia de Software | Unicatólica-TO
</p>

---
## 😆 Integrantes
- João Victor Ferreira Costa
- Eville Vitória Nunes Coelho

## 📝 Descrição do Projeto

O objetivo desta atividade foi analisar um código legado ("Big Ball of Mud"), identificar suas regras de negócio implícitas e transformá-lo em uma solução seguindo boas práticas de desenvolvimento, como **Clean Code**, **SOLID** e **Arquitetura em Camadas**.

---
## 🔍 Parte 01: Engenharia Reversa

Após análise do código original, foram identificadas as seguintes características:

* **Responsabilidade da Classe:** Gerenciar o fluxo completo de um pedido, desde a validação inicial até o cálculo de taxas, geração de logs e envio de notificações.
* **Problemas Identificados:**
    * **Método Gigante:** O método `ProcessarPedido` acumulava todas as responsabilidades.
    * **Obsessão por Primitivos:** Uso excessivo de strings e bools para representar conceitos complexos (ex: tipos de cliente, países).
    * **Regras de Negócio Ocultas:** Cálculos de taxas de categorias ("ALIMENTO", "IMPORTADO") misturados com a lógica de fluxo.
    * **Dificuldade de Teste:** O código original era difícil de testar de forma unitária devido ao alto acoplamento.

---

## 📄 Parte 02: Redocumentação

A redocumentação foi realizada através de:
1.  **Comentários Técnicos:** Inseridos no código original para explicar a finalidade de cada parâmetro e bloco de decisão.
2.  **Renomeação Semântica:** Variáveis como `subtotal`, `desconto` e `juros` foram mantidas, mas a estrutura foi alterada para que seus nomes refletissem exatamente sua origem (ex: `CalcularDescontoTotal`).

---

## 🏗️ Parte 03: Reestruturação (Refatoração)

Para a reestruturação, optei por uma abordagem moderna utilizando **C# 12**, mantendo tudo em um **arquivo único** conforme requisito pedagógico, mas organizando-o logicamente por Namespaces e Tipos.

### Meliorias Implementadas:

* **Tipagem Forte (Enums):** Substituição de strings mágicas por `enum TipoCliente` e `enum FormaPagamento`, evitando erros de digitação.
* **Records:** Uso de `record` para entidades de dados (`Pedido`, `Cliente`), garantindo imutabilidade durante o processamento.
* **Switch Expressions:** Substituição de `if/else` aninhados por expressões switch, tornando a leitura de regras de frete e desconto muito mais clara.
* **Precisão Financeira:** Uso estrito do tipo `decimal` (sufixo `m`) para todos os cálculos monetários, evitando erros de arredondamento de ponto flutuante.
* **Responsabilidade Única (SRP):** A lógica de taxas extras por categoria foi movida para a classe `ItemPedido`, enquanto o cálculo de frete e juros foi isolado em métodos privados dentro do serviço.

### Exemplo de Melhoria (Cálculo de Juros):
**Antes:** `if` complexos com operadores lógicos.
**Depois:**
```csharp
return p.Pagamento.Parcelas switch {
    > 1 and <= 6 => subtotal * 0.02m,
    > 6 => subtotal * 0.05m,
    _ => 0m
};
