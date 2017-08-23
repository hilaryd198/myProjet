using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Final.Models
{
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Nom complet")]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Courriel { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        [DisplayName("Confirmation du mot de passe")]
        public string ConfirmPassword { get; set; }

        // Ajoute un nouvel utilisateur dans la base de données
        // Cette focntion est appelée lors de la création d'un compte d'utilisateur
        public static bool creer(Utilisateur u)
        {
            bool TEST = true;

            byte[] hashPassword = new UTF8Encoding().GetBytes(u.Password);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(hashPassword);
            string hashString = BitConverter.ToString(hash);

            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "INSERT INTO Utilisateur (FullName, Courriel, Password) VALUES ('" + u.FullName + "', '" + u.Courriel + "', '" + hashString + "')";
            SqlConnection connexion = new SqlConnection(chConnexion);
            SqlCommand commande = new SqlCommand(requete, connexion);
            commande.CommandType = System.Data.CommandType.Text;

            try
            {
                connexion.Open();
                commande.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                TEST = false;
            }
            finally
            {
                connexion.Close();
            }
            return TEST;
        }

        // Cette méthode renvoie le nom complet d'un utilisateur à partir de son nom d'utilisateur.
        // Elle est utilisée car le cookie est créé avec le nom complet, ceci permet un plus bel affichage
        public static string getByUserName(string UserName)
        {
            string cStr = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            using (SqlConnection cnx = new SqlConnection(cStr))
            {
                string requete = "SELECT * FROM Utilisateur WHERE Courriel = '" + UserName + "'";
                SqlCommand cmd = new SqlCommand(requete, cnx);
                cmd.CommandType = System.Data.CommandType.Text;

                try
                {
                    cnx.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        dr.Close();
                        return null;
                    }
                    dr.Read();
                    return (string)dr["FullName"];
                }
                finally
                {
                    cnx.Close();
                }
            }
        }

        // Méthode permettant de comparer le login et le mot de passe fourni par l'utilisateur avec ce qui est stocké dans la base de données.
        public static bool Authentifie(string login, string passwd)
        {
            string cStr = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            using (SqlConnection cnx = new SqlConnection(cStr))
            {
                string requete = "SELECT * FROM Utilisateur WHERE Courriel = '" + login + "'";
                SqlCommand cmd = new SqlCommand(requete, cnx);
                cmd.CommandType = System.Data.CommandType.Text;

                try
                {
                    cnx.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (!dataReader.HasRows)
                    {
                        dataReader.Close();
                        return false;
                    }
                    dataReader.Read();
                    var encodedPasswordOnServer = (string)dataReader["Password"];

                    byte[] encodedPassword = new UTF8Encoding().GetBytes(passwd);
                    byte[] hash =
                    ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                    string encodedPasswordSentToForm = BitConverter.ToString(hash);
                    dataReader.Close();
                    return encodedPasswordSentToForm == encodedPasswordOnServer.Trim();
                }
                finally
                {
                    cnx.Close();
                }
            }
        }

        // Renvoie l'utilisateur complet à partir de son nom complet.
        // Utilisée car c'est le nom complet qui est stocké dans le cookie.
        public static Utilisateur getByFullName(string name)
        {
            string cStr = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            using (SqlConnection cnx = new SqlConnection(cStr))
            {
                string requete = "SELECT * FROM Utilisateur WHERE FullName = '" + name + "'";
                SqlCommand cmd = new SqlCommand(requete, cnx);
                cmd.CommandType = System.Data.CommandType.Text;

                try
                {
                    cnx.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        dr.Close();
                    }
                    dr.Read();
                    Utilisateur u = new Utilisateur { FullName = name, Id = (int)dr["Id"], Courriel = (string)dr["Courriel"] };
                    dr.Close();
                    return u;
                }
                finally
                {
                    cnx.Close();
                }
            }
        }

        // Permet de modifier le mot de passe de l'utilisateur.
        public static bool Modifier(Utilisateur u)
        {
            bool TEST = true;
            byte[] encodedPassword = new UTF8Encoding().GetBytes(u.Password);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
            string hashString = BitConverter.ToString(hash);
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            SqlConnection connexion = new SqlConnection(chConnexion);
            string requete = "UPDATE Utilisateur SET FullName = '" + u.FullName + "', Password = '" + hashString + "' WHERE Courriel = '" + u.Courriel + "'";
            SqlCommand commande = new SqlCommand(requete, connexion);
            commande.CommandType = System.Data.CommandType.Text;

            try
            {
                connexion.Open();
                commande.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string Message = e.Message;
                TEST = false;
            }
            finally
            {
                connexion.Close();
            }
            return TEST;
        }
    }
}