using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApi.Models;
using System.Data.SqlClient;
using System.Data;

namespace WebApi.Controllers

{
    public class EmployeeController : ControllerBase

    {
        //Inicializa la variable de la cadena de conexion
        private readonly string cadenaSQL;
        //constructor del controlador 
        public EmployeeController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }


        //Listar los employee
        //Atributos del metodo
        [HttpGet]
        [Route("Lista")]
        //metodo lista
        public IActionResult Lista()
        {
            //Inicializar una lista vacia de employee
            List<EmployeeEntity> lista = new List<EmployeeEntity>();
            try
            {
                //devolver una lista de empleados
                //conexion a la base de datos
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //abre la conexion con la db
                    conexion.Open();
                    //Ejecutar procedimiento almacenado sp_lis_employee
                    var cmd = new SqlCommand("sp_list_employeer", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //leer los resultados
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lista.Add(new EmployeeEntity()
                            {
                                IdEmployee = Convert.ToInt32(rd["IIdEmployeed"]),
                                NameEmployee = rd["NameEmployee"].ToString(),
                                Position = rd["Position"].ToString(),
                                Office = rd["Office"].ToString(),
                                Salary = Convert.ToDecimal(rd["Salary"])
                            });
                        }
                    }
                }
                //Devolver respuesta http
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Ok", Response = lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new {mensaje=error.Message,Response = lista});
            }
        }

        }
    }
