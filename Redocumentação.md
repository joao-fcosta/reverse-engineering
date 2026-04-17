## Parâmetros do Método `ProcessarPedido`

- **pedidoId**  
  Identificador único do pedido  
  _Validação:_ deve ser > 0

- **nomeCliente**  
  Nome completo para registro e log

- **emailCliente**  
  Endereço para envio de notificações  
  _Obrigatório se_ `enviarEmail = true`

- **tipoCliente**  
  Define a faixa de desconto base  
  _(VIP, PREMIUM, NORMAL, NOVO)_

- **itens**  
  Lista de objetos `ItemPedido` contendo produtos, preços e categorias

- **cupom**  
  Código alfanumérico para descontos extras ou frete grátis

- **formaPagamento**  
  Define se haverá:
  - Juros (`CARTAO`)
  - Descontos (`PIX` / `BOLETO`)

- **enderecoEntrega**  
  Local de destino  
  _Obrigatório para processamento_

- **pesoTotal**  
  Somatório do peso dos itens  
  _Define a faixa de preço do frete_

- **entregaExpressa**  
  Flag que adiciona taxa de urgência ao frete

- **clienteBloqueado**  
  Status de crédito do cliente  
  _Impede o processamento se `true`_

- **enviarEmail**  
  Gatilho para disparo de sistema de mensageria

- **salvarLog**  
  Gatilho para persistência de histórico em memória (`_logs`)

- **pais**  
  Sigla do país  
  - `BR` → ativa taxas nacionais  
  - Outros → taxas internacionais

- **parcelas**  
  Quantidade de vezes para pagamento  
  _Gera juros em `CARTAO` se > 1_
