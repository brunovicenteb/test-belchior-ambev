#Teste de Chave

# test-belchior-ambev
Projeto de avaliação para programador sênior da Ambev.

Projeto implementado em .Net 5 (net5.0) utilizando o Visual Studio Code juntamente com o bootstrap 5.1.0 para implentação e o plugin do Chrome Postman para testes;

Para fins de praticidade foi implementado usando SQLite e um banco com alguns dados segue anexado ao projeto;

Projeto inteiramente desenvolvido utilizando apenas o Visual Studio Code, passos para execução do projeto:

    #1 Visual Studio Code => Open Folder [Apontar para pasta raiz do projeto;]
    #2  Studio Code => Run => Run Build Task => build
    #3  Studio Code => Run => Run Without Debugging

Nesse ponto o seu browser padrão já deve aparecer com a aplicação Asp.Net Core rodando e a API já estará disponível na porta 5000;
Além da API existe um CRUD básico para Cliente/Cerveja e Vendas com um controle primário de estoque e entrega de vendas;
Não foi feito um cadastro das configurações de CashBack, todas elas já estão previamente cadastradas no Banco de Dados;


* Consultar o catálogo de cerveja de forma paginada, filtrando por nome e 
ordenando de forma crescente (Página arbitrada para dois registros);

http://{host}:{port}/api/beer/ConsultarCervejas?pagina=0&nome=skol
** Parâmetros fictícios mas funcionais
    
    Os dois parâmetros são opcionais;
    Caso nenhum parâmetro de página seja informada será adotada a página 0;
    O filtro de nome é uma espécie de like canse-insensitive nome like '%sk%';

----------------------------------------------------------------------------------

* Consultar a cerveja pelo seu identificador; 

http://{host}:{port}/api/beer/ConsultarCervejaPeloIdentificador?id=1
** Parâmetros fictícios mas funcionais

    Sem explicações necessárias;

----------------------------------------------------------------------------------

*   Consultar todas as vendas efetuadas de forma paginada, filtrando pelo range 
de datas (inicial e final) da venda e ordenando de forma decrescente pela data 
da venda;


http://{host}:{port}/api/beer/ConsultarVendas?inicio=2022-01-19T16:45:00.000Z&final=2022-01-19T16:47:00.000Z
** Parâmetros fictícios mas funcionais

    Os três parâmetros são opcionais;
    Caso nenhum parâmetro de página seja informada será adotada a página 0;

----------------------------------------------------------------------------------

* Consultar uma venda pelo seu identificador (Método Post, json da venda requisitado no corpo da requisição Http);

http://{host}:{port}/api/beer/RegistrarVendaDeCerveja
** Json Fictício mais funcional

{
   "idCliente": 11,
   "cliente":{
      "cpf":"760.947.580-72",
      "dataNascimento":"1985-04-30T00:00:00",
      "idUsuario":8,
      "nome": "Gabriela Gadotkovisk",
      "email":"gabriela.gadotkovisk@ambev.com.br"
   },
	"itensVenda":[
      {
         "idCerveja":1,
         "quantidade":5
      },
      {
         "idCerveja":4,
         "idCashBack":25,
      }
   ]
}


    A API sempre irá tentar reutilizar um cliente previamente cadastrado:
        Procurando primeiro por um cliente com o mesmo idCliente caso tenha sido informado
        Caso não ache, tenta buscar um cliente homônimo com mesmo e-mail ou CPF
        Caso nao encontre mesmo assim, cria um cliente novo com os dados informados no json
    Retorna um json completo da venda gerada (É possível visualizar ela posteriormente pela aplicação .Net Core)
