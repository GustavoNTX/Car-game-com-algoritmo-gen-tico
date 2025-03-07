Simulador de Corrida com Algoritmo Genético
Bem-vindo ao Simulador de Corrida com Algoritmo Genético! Este projeto é uma simulação de corrida onde carros autônomos evoluem ao longo de gerações, utilizando um algoritmo genético para melhorar seu desempenho. Abaixo, você encontrará uma explicação detalhada sobre como o jogo funciona, a lógica por trás do algoritmo genético, e como os carros aprendem a correr cada vez melhor.

Visão Geral do Jogo
O objetivo do jogo é criar uma população de carros autônomos que aprendem a completar um circuito de corrida. A cada geração, os carros são avaliados com base em seu desempenho (fitness), e os melhores são selecionados para gerar a próxima geração. O processo de evolução inclui seleção, crossover e mutação, permitindo que os carros se tornem mais eficientes ao longo do tempo.

Como Funciona o Jogo
1. População Inicial
No início do jogo, uma população de carros é criada. Cada carro é uma instância de um prefab (modelo de carro) e possui um conjunto de genes que definem seu comportamento.

Os genes são representados por um array de números reais (float[]), onde cada valor influencia a forma como o carro acelera, freia e vira.

2. Corrida e Avaliação
Os carros competem em um circuito com waypoints (checkpoints) que devem ser seguidos.

Durante a corrida, o desempenho de cada carro é medido por uma função de fitness, que leva em consideração:

Quantidade de waypoints alcançados.

Tempo para completar uma volta.

Velocidade média.

Carros que batem em paredes ou ficam parados por muito tempo são penalizados e removidos da corrida.

3. Evolução
Ao final de cada geração (após um tempo pré-definido ou quando todos os carros morrem), os melhores carros são selecionados para gerar a próxima geração.

O processo de evolução inclui:

Seleção: Os carros com maior fitness são escolhidos como pais.

Crossover: Os genes dos pais são combinados para criar novos carros (filhos).

Mutação: Pequenas alterações aleatórias são aplicadas aos genes dos filhos para introduzir diversidade.

4. Gerações Sucessivas
O processo se repete por várias gerações, com os carros se tornando cada vez mais eficientes em completar o circuito.

O jogo exibe informações como o número da geração atual, o melhor fitness e o tempo de volta.

Detalhes Técnicos
1. Algoritmo Genético
O algoritmo genético é o coração do jogo. Ele simula a evolução natural, onde os melhores indivíduos (carros) são selecionados para reproduzir e passar suas características para a próxima geração. Aqui está como ele funciona:

a. Representação dos Genes
Cada carro possui um array de genes (float[]), onde cada gene controla um aspecto do comportamento do carro, como aceleração, frenagem e direção.

Por exemplo:

genes[0]: Intensidade da aceleração.

genes[1]: Intensidade da rotação.

b. Fitness
A função de fitness avalia o desempenho de cada carro com base em:

Número de waypoints alcançados.

Tempo para completar uma volta.

Velocidade média.

Carros que batem em paredes ou ficam parados por muito tempo recebem uma penalidade no fitness.

c. Seleção
Os carros são ordenados por fitness, e os melhores são selecionados como pais para a próxima geração.

Um método comum usado é o elitismo, onde os melhores carros são mantidos intactos na próxima geração.

d. Crossover
O crossover combina os genes de dois pais para criar filhos. Neste projeto, é utilizado o crossover uniforme, onde cada gene do filho tem 50% de chance de vir do pai A ou do pai B.

Exemplo:

Pai A: [0.1, 0.5, 0.3]

Pai B: [0.4, 0.2, 0.7]

Filho: [0.1, 0.2, 0.7] (genes selecionados aleatoriamente).

e. Mutação
Após o crossover, os genes dos filhos podem sofrer mutações. A mutação introduz diversidade na população, permitindo que novos comportamentos sejam explorados.

A taxa de mutação (mutationRate) define a probabilidade de um gene ser alterado. Por exemplo:

Gene original: 0.5

Após mutação: 0.8 (valor aleatório entre -1 e 1).

2. Controle dos Carros
Cada carro é controlado por um script (CarBrain) que utiliza seus genes para tomar decisões, como:

Acelerar ou frear.

Virar para a esquerda ou direita.

O comportamento é ajustado dinamicamente com base nos genes e na detecção de obstáculos (usando raycasts).

3. Interface do Usuário
O jogo exibe informações importantes na tela, como:

Número da geração atual.

Melhor fitness da geração.

Tempo de volta do melhor carro.

Essas informações ajudam a acompanhar o progresso da evolução.
