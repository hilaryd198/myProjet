using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Final.Models
{
    // La classe Passager fait le lien entre les voyages et les utilisateurs.
    // Un voyage peut contenir plusieurs passagers
    // Un passager peut participer < plusieurs voyages
    public class Passager
    {
        public int IdUtilisateur { get; set; }
        public List<Voyage> Voyages { get; set; }

        public Passager()
        {
            Voyages = new List<Voyage>();            
        }

         
        // Cette méthode permet d'annuler la participation d'un passager < un voyage.
        // Le nombre de places disponibles pour ce voyage est incrémenté de 1
        public static bool Delete(int id)
        {
            string chConnexion = ConfigurationManager.ConnectionStrings["maCon"].ConnectionString;
            string requete = "DELETE FROM Passager WHERE IdVoyage = " + id;
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
            }
            finally
            {
                cnx.Close();
            }

            requete = "SELECT nbPlace FROM Voyage WHERE IdVoyage=" + id;
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

            nb = nb + 1;
            requete = "UPDATE Voyage SET nbPlace = " + nb + " WHERE IdVoyage = " + id;
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
    }
}