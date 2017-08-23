using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Final.Models
{
    public class Voyage
    {
        public int IdVoyage { get; set; }
        [DisplayName("Départ")]
        public string Depart { get; set; }
        [DisplayName("Arrivée")]
        public string Arrivee { get; set; }
        public DateTime Date { get; set; }
        public double Prix { get; set; }
        [DisplayName("Nombre de places")]
        public int nbPlace { get; set; }
        public int Conducteur { get; set; }

        public List<Utilisateur> Passagers { get; set; }

        public Voyage()
        {
            Passagers = new List<Utilisateur>();
        }

        // Renvoie la liste des voyages offerts par un conducteur
        // Affiche uniquement les voyages du conducteur dont le nom est passé en paramètre.
        public static List<Voyage> getVoyageParConducteur(string name)
        {
            List<Voyage> lesVoyages = new List<Voyage>();
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "SELECT * FROM Voyage, Utilisateur WHERE Voyage.Conducteur = Utilisateur.Id AND Utilisateur.FullName = '" + name + "'";
            SqlConnection cnx = new SqlConnection(chConnexion);
            SqlCommand cmd = new SqlCommand(requete, cnx);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Voyage v = new Voyage { Depart = (string)dr["Depart"], Arrivee = (string)dr["Arrivee"], Conducteur = (int)dr["Conducteur"], Date = (DateTime)dr["Date"], IdVoyage = (int)dr["IdVoyage"], nbPlace = (int)dr["nbPlace"], Prix = (double)dr["Prix"] };
                    lesVoyages.Add(v);
                }
                dr.Close();
                return lesVoyages;
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
            finally
            {
                cnx.Close();
            }
            return null;
        }

        // Renvoie la liste de tous les voyages disponibles pour que le passager puisse choisir parmi les voyages.
        public static List<Voyage> getAllVoyages()
        {
            List<Voyage> lesVoyages = new List<Voyage>();
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "SELECT * FROM Voyage";
            SqlConnection cnx = new SqlConnection(chConnexion);
            SqlCommand cmd = new SqlCommand(requete, cnx);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Voyage v = new Voyage { Depart = (string)dr["Depart"], Arrivee = (string)dr["Arrivee"], Conducteur = (int)dr["Conducteur"], Date = (DateTime)dr["Date"], IdVoyage = (int)dr["IdVoyage"], nbPlace = (int)dr["nbPlace"], Prix = (double)dr["Prix"] };
                    lesVoyages.Add(v);
                }
                dr.Close();
                return lesVoyages;
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
            finally
            {
                cnx.Close();
            }
            return null;
        }

        // Renvoie la liste de tous les voyages auquel un passager participe
        // Il s'agit du passager dont le nom est passé en paramètre
        public static List<Voyage> getVoyageParPassager(string name)
        {
            List<Voyage> lesVoyages = new List<Voyage>();
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "SELECT * FROM Voyage, Utilisateur, Passager WHERE Passager.IdVoyage = Voyage.IdVoyage AND Passager.IdUtilisateur = Utilisateur.Id AND Utilisateur.FullName = '" + name + "'";
            SqlConnection cnx = new SqlConnection(chConnexion);
            SqlCommand cmd = new SqlCommand(requete, cnx);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Voyage v = new Voyage { Depart = (string)dr["Depart"], Arrivee = (string)dr["Arrivee"], Conducteur = (int)dr["Conducteur"], Date = (DateTime)dr["Date"], IdVoyage = (int)dr["IdVoyage"], nbPlace = (int)dr["nbPlace"], Prix = (double)dr["Prix"] };
                    lesVoyages.Add(v);
                }
                dr.Close();
                return lesVoyages;
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
            finally
            {
                cnx.Close();
            }
            return null;
        }

        // Ajoute un nouveau voyage < la liste des voyages
        public static bool creer(Voyage v)
        {
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "INSERT INTO Voyage (Depart, Arrivee, Date, Prix, nbPlace, Conducteur) VALUES ('" + v.Depart + "', '" + v.Arrivee + "', '" + v.Date + "', " + v.Prix + ", " + v.nbPlace + ", " + v.Conducteur + ")";
            SqlConnection cnx = new SqlConnection(chConnexion);
            SqlCommand cmd = new SqlCommand(requete, cnx);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return false;
            }
            finally
            {
                cnx.Close();
            }
        }

        // Permet à un passager de choisir un voyage. 
        // Le voyage est ajouté à la liste des voyages du passager
        // Enlève une place au nombre de places disponibles
        public static bool Choisir(int IdVoyage, int IdPassager)
        {
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "INSERT INTO Passager (IdVoyage, IdUtilisateur) VALUES (" + IdVoyage +", " + IdPassager + ")";
            SqlConnection cnx = new SqlConnection(chConnexion);
            SqlCommand cmd = new SqlCommand(requete, cnx);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return false;
            }
            finally
            {
                cnx.Close();
            }

            requete = "SELECT nbPlace FROM Voyage WHERE IdVoyage=" + IdVoyage;
            SqlConnection cnx2 = new SqlConnection(chConnexion);
            SqlCommand cmd2 = new SqlCommand(requete, cnx2);
            cmd2.CommandType = System.Data.CommandType.Text;
            int nb;
            try
            {
                cnx2.Open();
                SqlDataReader dr = cmd2.ExecuteReader();
                dr.Read();
                nb = (int)dr["nbPlace"];
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return false;
            }
            finally
            {
                cnx2.Close();
            }

            nb = nb - 1;
            requete = "UPDATE Voyage SET nbPlace = " + nb + " WHERE IdVoyage = " + IdVoyage;
            SqlConnection cnx3 = new SqlConnection(chConnexion);
            SqlCommand cmd3 = new SqlCommand(requete, cnx3);
            cmd3.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx3.Open();
                cmd3.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return false;
            }
            finally
            {
                cnx3.Close();
            }
            return true;
        }

        // Renvoie un voyage en fonction de l'id
        public static Voyage getVoyageById(int id)
        {
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "SELECT * FROM Voyage WHERE IdVoyage = " + id;
            SqlConnection cnx = new SqlConnection(chConnexion);
            SqlCommand cmd = new SqlCommand(requete, cnx);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Voyage v = new Voyage { Depart = (string)dr["Depart"], Arrivee = (string)dr["Arrivee"], Conducteur = (int)dr["Conducteur"], Date = (DateTime)dr["Date"], IdVoyage = (int)dr["IdVoyage"], nbPlace = (int)dr["nbPlace"], Prix = (double)dr["Prix"] };
                dr.Close();
                return v;
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
            finally
            {
                cnx.Close();
            }
            return null;
        }

        // Supprime un voyge < partir de son id
        public static bool Supprimer(int id)
        {
            Passager.Delete(id);

            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "DELETE FROM Voyage WHERE IdVoyage = " + id;
            SqlConnection cnx = new SqlConnection(chConnexion);
            SqlCommand cmd = new SqlCommand(requete, cnx);
            cmd.CommandType = System.Data.CommandType.Text;
            try
            {
                cnx.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return false;
            }
            finally
            {
                cnx.Close();
            }
        }
    }
}