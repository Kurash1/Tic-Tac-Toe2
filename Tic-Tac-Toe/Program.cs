//Tämän työn saa tehdä millä tahansa kiellä, ainut requirement on että se on console/terminal app
//Tee peli, alku peräsestä ideasta Tic-Tac-Toe.
//Tämä tarkoittaa että alkakaa tekemään Tic-Tac-Toe ja jos on ideoita kesken sen tekemistä, vaikka entä jos se olisi 4x4 mielummin, niin yrittäkää tehdä se ja katsokaa onko se hauskaa.
//
//Muuten ei ole kunnollisia sääntöjä, pelin teossa tärkeä asia minun mielestä on sanonta "Follow the Fun",
//joka tairkottaa että tehkää sitä peliä, ja kun tulee ideoita ja yritä ja katso tekeekö se siitä hauskemman.
//
//Extra haastetta on, tekeminen ai-vihollinen pelaajalle, ja/tai asetuksia pelille.

using Pastel;
using Tic_Tac_Toe;
using ExtensionMethods;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Http.Headers;

retry2:
string nettype = Read("Host or Client?").ToUpper();
TcpClient client;
if (nettype.StartsWith("HOST"))
{
    TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(Read("Please Input a port")));
    listener.Start();
    client = listener.AcceptTcpClient();
    Console.WriteLine("Connection Established");
}
else if (nettype.StartsWith("CLIENT"))
{
    client = new TcpClient(Read("Please input ip"), int.Parse(Read("Please Input Port")));
    Console.WriteLine("Connection Established");
}
else
    goto retry2;
NetworkStream stream = client.GetStream();
int BoardWidth = 9;
int BoardHeight = 9;
bool game = true;
spot[,] board = new spot[BoardWidth, BoardHeight];
player none = new player(ConsoleColor.White, ' ');
for (int x = 0; x < BoardWidth; x++)
{
    for(int y = 0; y < BoardHeight; y++)
    {
        board[x, y] = new spot(none);
    }
}
player[] Players = new player[]
{
    new player(ConsoleColor.Red,    'R', board[0,0]),
    new player(ConsoleColor.Yellow, 'Y', board[BoardWidth-1,BoardHeight-1])
};
for(int i = 0; i < Players.Length; i++)
{
    Players[i].capital.owner = Players[i];
}

int move = 0;
string end = "";
while (game)
{
    for(int i = 0; i < Players.Length && game; i++)
    {
        move++;
        draw(Players[i]);
        spot pos;
        if (nettype == "HOST")
            pos = getOnlineInput(stream);
        else
            pos = getInput();
        pos.owner = Players[i];
        player? win = Winner();
        if(win != null)
        {
            end = $"Player {win.character} has Won".Pastel(win.color);
            game = false;
        }
        if (move >= BoardHeight * BoardWidth)
        {
            end = "Draw";
            game = false;
        }
    }
}
draw();
Console.WriteLine(end);
player? Winner()
{
    //rows
    for(int y = 0; y < BoardHeight; y++)
    {
        bool isOver = true;
        player cur = board[0,y].owner;
        if (cur == none)
            continue;
        for(int x = 0; x < BoardWidth; x++)
        {
            if (board[x,y].owner != cur)
                isOver = false;
        }
        if(isOver)
            return cur;
    }
    //columns
    for (int x = 0; x < BoardWidth; x++)
    {
        bool isOver = true;
        player cur = board[x, 0].owner;
        if (cur == none)
            continue;
        for (int y = 0; y < BoardHeight; y++)
        {
            if (board[x, y].owner != cur)
                isOver = false;
        }
        if (isOver)
            return cur;
    }
    //diagonals TODO: make dynamic, aka make work with any size
    if (board[0, 0].owner != none && board[0, 0].owner == board[1, 1].owner && board[1, 1].owner == board[2, 2].owner)
        return board[1,1].owner;
    if (board[2, 0].owner != none && board[2, 0].owner == board[1, 1].owner && board[1, 1].owner == board[0, 2].owner)
        return board[1, 1].owner;
    return null;
}
spot getOnlineInput(Stream stream)
{
    retry:
    byte[] data = new byte[1024];
    int bytesread = stream.Read(data, 0, data.Length);
    string? trueInput = Encoding.ASCII.GetString(data, 0, bytesread);
    string[] input = trueInput.Split(' ');
    if (!int.TryParse(input[0], out int x))
        goto retry;
    x--;
    if (!int.TryParse(input[1], out int y))
        goto retry;
    y--;
    return board[x, y];
}
spot getInput()
{
retrylabel:
    string? trueInput = Console.ReadLine();
    if (trueInput == null)
        goto retry;
    string[] input = trueInput.Split(' ');
    if (input.Length != 2)
        goto retry;
    if (!int.TryParse(input[0], out int x))
        goto retry;
    x--;
    if (!int.TryParse(input[1], out int y))
        goto retry;
    y--;
    if (!x.inRange(0, 2))
        goto retry;
    if (!y.inRange(0, 2))
        goto retry;
    if (board[x,y].owner != none)
        goto retry;
    return board[x, y];
retry:
    Console.WriteLine("Invalid, Please Retry");
    goto retrylabel;
}
void draw(player? player = null)
{
    Console.Clear();
    if (player != null)
        Console.Write($"Player {player.character} turn".Pastel(player.color));
    Console.WriteLine(" Input: {x} {y}");
    Console.Write(' ');
    for (int x = 0; x < BoardWidth; x++)
    {
        Console.Write($" {x + 1}");
    }
    Console.Write('\n');
    for (int y = 0; y < BoardHeight; y++)
    {
        Console.Write(y + 1);
        for (int x = 0; x < BoardWidth; x++)
        {
            Console.Write($" {board[x, y].owner.character.ToString().Pastel(board[x, y].owner.color)}");
        }
        Console.Write('\n');
    }
}
string? Read(string message = "")
{
    if (message != "")
        Console.WriteLine(message);
    return Console.ReadLine();
}