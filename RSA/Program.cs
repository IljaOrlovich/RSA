using System;
using System.IO;

public class RSA
{
    private static Random random = new Random();

    public static void Main()
    {
        bool repeat = true;

        while (repeat)
        {
            Console.WriteLine("Pasirinkite veiksmą (1 - šifravimas, 2 - dešifravimas, 0 - baigti):");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 0)
            {
                repeat = false;
                continue;
            }

            if (choice == 1)
            {
                EncryptMessage();
            }
            else if (choice == 2)
            {
                DecryptMessage();
            }
        }
    }


    private static void EncryptMessage()
    {
        Console.WriteLine("Įveskite pirminį skaičių p:");
        int p = int.Parse(Console.ReadLine());
        Console.WriteLine("Įveskite pirminį skaičių q:");
        int q = int.Parse(Console.ReadLine());
        Console.WriteLine("Įveskite pradinį tekstą:");
        string plaintext = Console.ReadLine();

        int n = p * q;
        int phi = (p - 1) * (q - 1);
        int e = GenerateE(phi);
        int d = GenerateD(phi, e);

        string encryptedText = Encrypt(plaintext, n, e);
        Console.WriteLine("Užšifruotas tekstas: " + encryptedText);
        SaveToFile("encrypted_text.txt", encryptedText);
        SaveKeysToFile("keys.txt", n, e, d);
    }

    private static void DecryptMessage()
    {
        string fileName = "keys.txt";
        try
        {
            string[] lines = File.ReadAllLines(fileName);
            int n = int.Parse(lines[0].Split(';')[0]); 
            int e = int.Parse(lines[0].Split(';')[1]);
            int d = int.Parse(lines[1]);

            fileName = "encrypted_text.txt";
            string encryptedText = File.ReadAllText(fileName);

            int p = FindPrimeFactor(n); // Surandame p, pirminį veiksnį
            int q = n / p; // Apskaičiuojame q
            int phi = (p - 1) * (q - 1); // Eulerio funkcija φ(n)

            string decryptedText = Decrypt(encryptedText, n, d);
            Console.WriteLine("Dešifruotas tekstas: " + decryptedText);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Klaida skaitant failą: " + ex.Message);
        }
    }

    // Metodas išsaugoti tekstą į failą
    private static void SaveToFile(string fileName, string text)
    {
        Console.WriteLine("Ar norite išsaugoti rezultatą į failą? (y/n):");
        if (Console.ReadLine().ToLower() == "y")
        {
            File.WriteAllText(fileName, text);
            Console.WriteLine("Rezultatas išsaugotas į failą: " + fileName);
        }
    }

    // Metodas išsaugoti viešąjį ir privačiąjį raktus į failą
    private static void SaveKeysToFile(string fileName, int n, int e, int d)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            writer.WriteLine(n + ";" + e);
            writer.WriteLine(d);
        }
        Console.WriteLine("Raktai išsaugoti į failą: " + fileName);
    }

    // Metodas sugeneruoti viešąjį raktą e
    private static int GenerateE(int phi)
    {
        int e = 2;
        while (GCD(e, phi) != 1)
        {
            e++;
        }
        return e;
    }

    // Metodas sugeneruoti privačiąjį raktą d
    private static int GenerateD(int phi, int e)
    {
        int d = 0;
        int k = 1;
        while ((k * phi + 1) % e != 0)
        {
            k++;
        }
        d = (k * phi + 1) / e;
        return d;
    }

    // Metodas šifruoti tekstą pagal RSA algoritmą
    private static string Encrypt(string plaintext, int n, int e)
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(plaintext);
        int[] encryptedBytes = new int[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            encryptedBytes[i] = ModPow(bytes[i], e, n);
        }
        return string.Join(" ", encryptedBytes);
    }

    // Metodas dešifruoti tekstą pagal RSA algoritmą
    private static string Decrypt(string encryptedText, int n, int d)
    {
        string[] encryptedChars = encryptedText.Split(' ');
        char[] decryptedChars = new char[encryptedChars.Length];
        for (int i = 0; i < encryptedChars.Length; i++)
        {
            decryptedChars[i] = (char)ModPow(int.Parse(encryptedChars[i]), d, n);
        }
        return new string(decryptedChars);
    }

    // Metodas skaičiuoti galingumą pagal modulį
    private static int ModPow(int baseNumber, int exponent, int modulus)
    {
        int result = 1;
        while (exponent > 0)
        {
            if (exponent % 2 == 1)
            {
                result = (result * baseNumber) % modulus;
            }
            exponent >>= 1;
            baseNumber = (baseNumber * baseNumber) % modulus;
        }
        return result;
    }

    // Metodas rasti Didžiausią Bendrą Daliklį (DBD) naudojant Euklido algoritmą
    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    // Metodas rasti pirminį skaičių veiksnį
    private static int FindPrimeFactor(int n)
    {
        for (int i = 2; i < n; i++)
        {
            if (n % i == 0 && IsPrime(i))
            {
                return i;
            }
        }
        return 0;
    }

    // Metodas tikrinti, ar skaičius yra pirminis
    private static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number <= 3) return true;

        if (number % 2 == 0 || number % 3 == 0) return false;

        for (int i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
            {
                return false;
            }
        }
        return true;
    }
}
