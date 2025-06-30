using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace CasinoInfo_bot
{
    internal class Bet
    {
        public int bet { get; set; }
        public int C1 { get; set; }
        public int C5 { get; set; }
        public int C25 { get; set; }
        public int C50 { get; set; }
        public int C100 { get; set; }
        public int C500 { get; set; }
        public int C1000 { get; set; }



        string conect = Security.MySQL_Connection;

        public async Task SelectAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            { 
                await connection.OpenAsync();

                string comanda = $"SELECT * FROM bet WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();
                        this.bet = Convert.ToInt32(read["bet"]);
                        this.C1 = Convert.ToInt32(read["1"]);
                        this.C5 = Convert.ToInt32(read["5"]);
                        this.C25 = Convert.ToInt32(read["25"]);
                        this.C50 = Convert.ToInt32(read["50"]);
                        this.C100 = Convert.ToInt32(read["100"]);
                        this.C500 = Convert.ToInt32(read["500"]);
                        this.C1000 = Convert.ToInt32(read["1000"]);
                        read.Close();
                    }

                }

            }
        }
        public async Task clearBet(long id)
        {

            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                
                await SelectAsync(id);
                if (bet != 0)
                {

                    var comanda = $"UPDATE acaunt SET `1` = `1` + {C1}, `5` = `5` + {C5}, `25` = `25` + {C25}, `50` = `50` + {C50}, `100` = `100` + {C100}, `500` = `500` + {C500}, `1000` = `1000` + {C1000} WHERE ID = @ID";
                    using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                    {
                        zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = id;
                        await zapros.ExecuteNonQueryAsync();
                    }
                    await ResetBetAsync(id);

                }
            }
        }
        public async Task SelectBetAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string comanda = $"SELECT bet FROM bet WHERE ID = @ID;";
                using (MySqlCommand zapros = new MySqlCommand(comanda, connection))
                {
                    zapros.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    using (var read = await zapros.ExecuteReaderAsync())
                    {
                        read.Read();
                        this.bet = Convert.ToInt32(read["bet"]);
                        
                        read.Close();
                    }

                }

            }
        }
        public async Task ResetBetAsync(long UserId)
        {
            using (var connection = new MySqlConnection(conect))
            {
                await connection.OpenAsync();

                string updateCommand = @"
            UPDATE bet 
            SET bet = 0, 
                `1` = 0, 
                `5` = 0, 
                `25` = 0, 
                `50` = 0, 
                `100` = 0, 
                `500` = 0, 
                `1000` = 0
            WHERE ID = @ID;";

                using (MySqlCommand command = new MySqlCommand(updateCommand, connection))
                {
                    command.Parameters.Add("@ID", MySqlDbType.Int64).Value = UserId;

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
