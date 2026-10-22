using System.Globalization;

// DADOS
struct carro
{
    public string modelo;
    public string cor;
    public int quantidade;
    public decimal preco;
}

struct venda
{
    public string modelo;
    public string cor;
    public int quantidadeVendida;
    public decimal precoUnitario;
}

class Program
{
    static List<carro> carros = new List<carro>();
    static List<venda> vendas = new List<venda>();

// PERSISTENCIA
    static void salvarCarros(List<carro> lista)
    {
        using StreamWriter arquivo = new StreamWriter("carros.txt");
        foreach (carro item in lista)
            arquivo.WriteLine($"{item.modelo};{item.cor};{item.quantidade};{item.preco.ToString(CultureInfo.InvariantCulture)}");
    }

    static List<carro> carregarCarros()
    {
        List<carro> lista = new List<carro>();
        if (!File.Exists("carros.txt")) return lista;
        using StreamReader arquivo = new StreamReader("carros.txt");
        string? linha;
        while ((linha = arquivo.ReadLine()) != null)
        {
            string[] dados = linha.Split(';');
            if (dados.Length == 4 && int.TryParse(dados[2], out int quantidade) && decimal.TryParse(dados[3], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal preco))
                lista.Add(new carro { modelo = dados[0], cor = dados[1], quantidade = quantidade, preco = preco });
        }
        return lista;
    }

    static void salvarVendas(List<venda> lista)
    {
        using StreamWriter arquivo = new StreamWriter("vendas.txt");
        foreach (venda item in lista)
            arquivo.WriteLine($"{item.modelo};{item.cor};{item.quantidadeVendida};{item.precoUnitario.ToString(CultureInfo.InvariantCulture)}");
    }

    static List<venda> carregarVendas()
    {
        List<venda> lista = new List<venda>();
        if (!File.Exists("vendas.txt")) return lista;
        using StreamReader arquivo = new StreamReader("vendas.txt");
        string? linha;
        while ((linha = arquivo.ReadLine()) != null)
        {
            string[] dados = linha.Split(';');
            if (dados.Length == 4 && int.TryParse(dados[2], out int quantidade) && decimal.TryParse(dados[3], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal preco))
                lista.Add(new venda { modelo = dados[0], cor = dados[1], quantidadeVendida = quantidade, precoUnitario = preco });
        }
        return lista;
    }

// LOGICA
    static void cadastrarCarro()
    {
        Console.Write("Modelo: ");
        string modelo = Console.ReadLine() ?? "";
        Console.Write("Cor: ");
        string cor = Console.ReadLine() ?? "";
        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade < 0) { Console.WriteLine("Dados invalidos."); return; }
        Console.Write("Preco: ");
        if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal preco) || preco < 0) { Console.WriteLine("Dados invalidos."); return; }
        carros.Add(new carro { modelo = modelo, cor = cor, quantidade = quantidade, preco = preco });
        salvarCarros(carros);
        Console.WriteLine("Carro cadastrado.");
    }

    static void consultarCarro(string modelo)
    {
        carro? encontrado = carros.Find(item => item.modelo.Equals(modelo, StringComparison.OrdinalIgnoreCase));
        if (encontrado == null) { Console.WriteLine("Carro nao encontrado."); return; }
        carro item = encontrado.Value;
        Console.WriteLine($"{item.modelo} | {item.cor} | Estoque: {item.quantidade} | R$ {item.preco:F2}");
    }

    static void excluirCarro(string modelo)
    {
        int indice = carros.FindIndex(item => item.modelo.Equals(modelo, StringComparison.OrdinalIgnoreCase));
        if (indice < 0) { Console.WriteLine("Carro nao encontrado."); return; }
        carros.RemoveAt(indice);
        salvarCarros(carros);
        Console.WriteLine("Carro excluido.");
    }

    static void realizarVendaCarro(string modelo, int quantidade)
    {
        int indice = carros.FindIndex(item => item.modelo.Equals(modelo, StringComparison.OrdinalIgnoreCase));
        if (indice < 0) { Console.WriteLine("Carro nao encontrado."); return; }
        if (quantidade <= 0 || carros[indice].quantidade < quantidade) { Console.WriteLine("Estoque insuficiente."); return; }
        carro item = carros[indice];
        item.quantidade -= quantidade;
        carros[indice] = item;
        vendas.Add(new venda { modelo = item.modelo, cor = item.cor, quantidadeVendida = quantidade, precoUnitario = item.preco });
        salvarCarros(carros);
        salvarVendas(vendas);
        Console.WriteLine("Venda realizada.");
    }

    static void calcularTotaisDeVenda()
    {
        int quantidade = 0;
        decimal total = 0;
        foreach (venda item in vendas)
        {
            quantidade += item.quantidadeVendida;
            total += item.quantidadeVendida * item.precoUnitario;
        }
        Console.WriteLine($"Quantidade vendida: {quantidade}");
        Console.WriteLine($"Valor total: R$ {total:F2}");
    }

// INTERFACE
    static void menuCadastro()
    {
        string opcao;
        do
        {
            Console.WriteLine("\n1 - Cadastrar carro\n0 - Voltar");
            opcao = Console.ReadLine() ?? "";
            switch (opcao) { case "1": cadastrarCarro(); break; case "0": break; default: Console.WriteLine("Opcao invalida."); break; }
        } while (opcao != "0");
    }

    static void menuVendas()
    {
        string opcao;
        do
        {
            Console.WriteLine("\n1 - Realizar venda\n0 - Voltar");
            opcao = Console.ReadLine() ?? "";
            if (opcao == "1")
            {
                Console.Write("Modelo: ");
                string modelo = Console.ReadLine() ?? "";
                Console.Write("Quantidade: ");
                if (int.TryParse(Console.ReadLine(), out int quantidade)) realizarVendaCarro(modelo, quantidade);
                else Console.WriteLine("Quantidade invalida.");
            }
            else if (opcao != "0") Console.WriteLine("Opcao invalida.");
        } while (opcao != "0");
    }

    static void menuConsultaExclusao()
    {
        string opcao;
        do
        {
            Console.WriteLine("\n1 - Consultar carro\n2 - Excluir carro\n0 - Voltar");
            opcao = Console.ReadLine() ?? "";
            if (opcao == "1" || opcao == "2")
            {
                Console.Write("Modelo: ");
                string modelo = Console.ReadLine() ?? "";
                if (opcao == "1") consultarCarro(modelo); else excluirCarro(modelo);
            }
            else if (opcao != "0") Console.WriteLine("Opcao invalida.");
        } while (opcao != "0");
    }

    static void menuRelatorios()
    {
        string opcao;
        do
        {
            Console.WriteLine("\n1 - Totais de venda\n0 - Voltar");
            opcao = Console.ReadLine() ?? "";
            if (opcao == "1") calcularTotaisDeVenda();
            else if (opcao != "0") Console.WriteLine("Opcao invalida.");
        } while (opcao != "0");
    }

    static void Main()
    {
        carros = carregarCarros();
        vendas = carregarVendas();
        string opcao;
        do
        {
            Console.WriteLine("\n=== Loja de Carros ===\n1 - Cadastro\n2 - Vendas\n3 - Consulta/Exclusao\n4 - Relatorios\n0 - Sair");
            opcao = Console.ReadLine() ?? "";
            switch (opcao)
            {
                case "1": menuCadastro(); break;
                case "2": menuVendas(); break;
                case "3": menuConsultaExclusao(); break;
                case "4": menuRelatorios(); break;
                case "0": break;
                default: Console.WriteLine("Opcao invalida."); break;
            }
        } while (opcao != "0");
    }
}
