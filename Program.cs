using Bogus;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakerData
{
    class TipoProduto
    {
        public int CodTipoProduto { get; set; }
        public string Nome { get; set; }
    }

    class Cliente
    {
        public int CodCliente { get; set; }
        public string Nome { get; set; }
        public char Tipo { get; set; }
    }

    class Produto
    {
        public int CodProduto { get; set; }
        public string NomeProduto { get; set; }
        public int CodTipoProduto { get; set; }
        public string Descricao { get; set; }
        public double Preco { get; set; }
    }

    class Pedido
    {
        public int CodPedido { get; set; }
        public int CodCliente { get; set; }
        public double Total { get; set; }
        public DateTime DataPedido { get; set; }
        public bool Cancelado { get; set; }
    }

    class ItemPedido
    {
        public int CodItemPedido { get; set; }
        public int CodPedido { get; set; }
        public int Quantidade { get; set; }
        public double ValorCobrado { get; set; }
        public int CodProduto { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Performance;Integrated Security=true";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //Set the randomizer seed if you wish to generate repeatable data sets.
                Randomizer.Seed = new Random(8675309);

                Console.WriteLine("Iniciando\n");
                var codTipoProduto = 0;
                var fakerTipoProduto = new Faker<TipoProduto>("pt_BR")
                    .StrictMode(true)
                    .RuleFor(o => o.CodTipoProduto, f => codTipoProduto++)
                    .RuleFor(o => o.Nome, f => f.Commerce.Random.CollectionItem(f.Commerce.Categories(50)));

                var dataTipoProduto = fakerTipoProduto.Generate(1000);
                Console.WriteLine("Tipos de produtos criados.\n");

                foreach (var tipoProduto in dataTipoProduto)
                {
                    SqlCommand command = new SqlCommand($"INSERT INTO TipoProduto (CodTipoProduto, Nome) VALUES (@CodTipoProduto, @Nome)", connection);
                    command.Parameters.AddWithValue("CodTipoProduto", tipoProduto.CodTipoProduto);
                    command.Parameters.AddWithValue("Nome", tipoProduto.Nome);
                    command.ExecuteNonQuery();
                }
                Console.WriteLine("Insert tipo de produtos feitos.\n");

                var tipoCliente = new[] { 'F', 'J' };

                var cancelado = new[] { true, false };

                var codCliente = 0;
                var fakerCliente = new Faker<Cliente>("pt_BR")
                    .StrictMode(true)
                    .RuleFor(o => o.CodCliente, f => codCliente++)
                    .RuleFor(o => o.Nome, f => f.Name.FullName())
                    .RuleFor(o => o.Tipo, f => f.PickRandom(tipoCliente));

                var dataCliente = fakerCliente.Generate(5000);
                Console.WriteLine("Clientes criados.\n");

                foreach (var cliente in dataCliente)
                {
                    SqlCommand command = new SqlCommand($"INSERT INTO Cliente (CodCliente, Nome, Tipo) VALUES (@CodCliente, @Nome, @Tipo)", connection);
                    command.Parameters.AddWithValue("CodCliente", cliente.CodCliente);
                    command.Parameters.AddWithValue("Nome", cliente.Nome);
                    command.Parameters.AddWithValue("Tipo", cliente.Tipo);
                    command.ExecuteNonQuery();
                }
                Console.WriteLine("Insert clientes feitos.\n");

                var codProduto = 0;
                var fakerProduto = new Faker<Produto>("pt_BR")
                    .StrictMode(true)
                    .RuleFor(o => o.CodProduto, f => codProduto++)
                    .RuleFor(o => o.NomeProduto, f => f.Commerce.ProductName())
                    .RuleFor(o => o.CodTipoProduto, f => f.PickRandom(dataTipoProduto.Select(d => d.CodTipoProduto)))
                    .RuleFor(o => o.Descricao, f => f.Lorem.Sentences(10))
                    .RuleFor(o => o.Preco, f => (double)f.Finance.Amount(1, 50));

                var dataProduto = fakerProduto.Generate(5000);
                Console.WriteLine("Produtos criados.\n");

                foreach (var produto in dataProduto)
                {
                    SqlCommand command = new SqlCommand($"INSERT INTO Produto (CodProduto, NomeProduto, CodTipoProduto, Descricao, Preco) VALUES (@CodProduto, @NomeProduto, @CodTipoProduto, @Descricao, @Preco)", connection);
                    command.Parameters.AddWithValue("CodProduto", produto.CodProduto);
                    command.Parameters.AddWithValue("NomeProduto", produto.NomeProduto);
                    command.Parameters.AddWithValue("CodTipoProduto", produto.CodTipoProduto);
                    command.Parameters.AddWithValue("Descricao", produto.Descricao);
                    command.Parameters.AddWithValue("Preco", produto.Preco);
                    command.ExecuteNonQuery();
                }
                Console.WriteLine("Insert produtos feitos.\n");

                var i = 0;
                var codPedido = 0;
                var codItensPedido = 0;
                for (i = 0; i < 100; i++)
                {
                    var fakerPedido = new Faker<Pedido>("pt_BR")
                        .StrictMode(true)
                        .RuleFor(o => o.CodPedido, f => codPedido++)
                        .RuleFor(o => o.CodCliente, f => f.PickRandom(dataCliente.Select(d => d.CodCliente)))
                        .RuleFor(o => o.Total, f => (double)f.Finance.Amount(1, 1000))
                        .RuleFor(o => o.DataPedido, f => f.Date.Recent())
                        .RuleFor(o => o.Cancelado, f => f.PickRandom(cancelado));

                    var dataPedido = fakerPedido.Generate(50000);
                    Console.WriteLine("Pedidos criados.\n");

                    var fakerItensPedido = new Faker<ItemPedido>("pt_BR")
                                            .StrictMode(true)
                                            .RuleFor(o => o.CodItemPedido, f => codItensPedido++)
                                            .RuleFor(o => o.CodPedido, f => f.PickRandom(dataPedido.Select(d => d.CodPedido)))
                                            .RuleFor(o => o.ValorCobrado, f => (double)f.Finance.Amount(1, 1000))
                                            .RuleFor(o => o.Quantidade, f => f.Random.Int(1, 10))
                                            .RuleFor(o => o.CodProduto, f => f.PickRandom(dataProduto.Select(d => d.CodProduto)));

                    var dataItensPedido = fakerItensPedido.Generate(50000);
                    Console.WriteLine("Itens pedidos criados.\n");

                    foreach (var pedido in dataPedido)
                    {
                        SqlCommand command = new SqlCommand($"INSERT INTO Pedido (CodPedido, CodCliente, Total, DataPedido, Cancelado) VALUES (@CodPedido, @CodCliente, @Total, @DataPedido, @Cancelado)", connection);
                        command.Parameters.AddWithValue("CodPedido", pedido.CodPedido);
                        command.Parameters.AddWithValue("CodCliente", pedido.CodCliente);
                        command.Parameters.AddWithValue("Total", pedido.Total);
                        command.Parameters.AddWithValue("DataPedido", pedido.DataPedido);
                        command.Parameters.AddWithValue("Cancelado", pedido.Cancelado);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine($"Insert pedidos feitos. Pedido atual: {codPedido}.\n");

                    foreach (var itemPedido in dataItensPedido)
                    {
                        SqlCommand command = new SqlCommand($"INSERT INTO ItensPedido (CodItemPedido, CodPedido, Quantidade, ValorCobrado, CodProduto) VALUES (@CodItemPedido, @CodPedido, @Quantidade, @ValorCobrado, @CodProduto)", connection);
                        command.Parameters.AddWithValue("CodItemPedido", itemPedido.CodItemPedido);
                        command.Parameters.AddWithValue("CodPedido", itemPedido.CodPedido);
                        command.Parameters.AddWithValue("Quantidade", itemPedido.Quantidade);
                        command.Parameters.AddWithValue("ValorCobrado", itemPedido.ValorCobrado);
                        command.Parameters.AddWithValue("CodProduto", itemPedido.CodProduto);
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine($"Insert itens de pedidos feitos. Itens Pedido atual: {codPedido}\n");
                }
            }
        }
    }
}


//CREATE DATABASE Performance;
//GO

//USE Performance;
//GO


//CREATE TABLE TipoProduto
//(
//    CodTipoProduto INT PRIMARY KEY,
//    Nome VARCHAR(50) NOT NULL
//)

//CREATE TABLE Cliente
//(
//    CodCliente INT PRIMARY KEY,
//    Nome VARCHAR(50) NOT NULL,
//    Tipo CHAR NOT NULL
//)

//CREATE TABLE Produto
//(
//    CodProduto INT PRIMARY KEY,
//    NomeProduto VARCHAR(30),
//	CodTipoProduto INT NOT NULL REFERENCES TipoProduto(CodTipoProduto),
//	Descricao TEXT NOT NULL,
//    Preco MONEY NOT NULL
//)

//CREATE TABLE Pedido
//(
//    CodPedido INT PRIMARY KEY,
//    CodCliente INT NOT NULL REFERENCES Cliente(CodCliente),
//    Total MONEY NOT NULL,
//    DataPedido DATETIME NOT NULL,
//    Cancelado BIT DEFAULT 0 NOT NULL,
//)

//CREATE TABLE ItensPedido
//(
//    CodItemPedido INT PRIMARY KEY,
//    CodPedido INT NOT NULL REFERENCES Pedido(CodPedido),
//    Quantidade INT NOT NULL,
//    ValorCobrado MONEY NOT NULL,
//    CodProduto INT NOT NULL REFERENCES Produto(CodProduto)
//)