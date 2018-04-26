using Model;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{
    //classe da camada de acesso a dados para armazenar os dados de navegação no banco de dados
    public class NavigationDataDao
    {
        string cnnString = @"Server=LAPTOP-QMVPA40S\SQLEXPRESS;Database=DbMqNavData;Trusted_Connection=True;";

        /*
        tabela criada para armazenar os dados

        create table TbNavigationData
        (
            Id int primary key identity,
            Data varchar(max) not null,
            InsertDateTime datetime2 default getdate()
        )
        */
        public void Insert(Model.NavigationData navData) {
            using(SqlConnection cnn=new SqlConnection(cnnString))
            {
                string commandText = "INSERT INTO TbNavigationData (data) VALUES (@data)";

                using(SqlCommand command = new SqlCommand(commandText))
                {
                    command.Connection=cnn;
                    command.Parameters.Add("@data", SqlDbType.VarChar).Value = navData.Data;
                    cnn.Open();

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}