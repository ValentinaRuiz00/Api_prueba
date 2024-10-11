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
        [Route("ListaEmpleado")]
        //metodo lista
        public IActionResult ListaEmpleado()
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
                                IdEmployee = Convert.ToInt32(rd["IdEmployee"]),
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = lista });
            }
        }

        //listar employee por id
        //Atributos del metodo
        [HttpGet]
        [Route("ObtenerEmpleado/{IdEmployee:int}")]

        public IActionResult ObtenerEmpleado(int IdEmployee)
        {
            //Inicializar un objeto para almacenar los datos especificos que almacena
            EmployeeEntity employeeEntity = new EmployeeEntity();

            try
            {
                //Conexion a base de datos
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //abrir la conexion
                    conexion.Open();
                    //Ejecutar procedimiento almacenado sp_get_employee
                    var cmd = new SqlCommand("sp_get_employee", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //pasar el valor de IdEmployee al parámetro del procedimiento almacenado
                    cmd.Parameters.AddWithValue("@IdEmployee", IdEmployee);
                    //Leer datos
                    using (var rd = cmd.ExecuteReader())
                    {
                        //Si se encuentra el registro, lee los datos
                        if (rd.Read())
                        {
                            employeeEntity = new EmployeeEntity()
                            {
                                IdEmployee = Convert.ToInt32(rd["IdEmployee"]),
                                NameEmployee = rd["NameEmployee"].ToString(),
                                Position = rd["Position"].ToString(),
                                Office = rd["Office"].ToString(),
                                Salary = Convert.ToDecimal(rd["Salary"])
                            };
                        }
                    }
                    //devolver el resultado si se encontró
                    if (employeeEntity != null)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { mensaje = "Ok", Response = employeeEntity });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Empleado no encontrado", Response = employeeEntity });
                    }
                }
            }
            catch (Exception error)
            {
                //si hay un error
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = employeeEntity });
            }
        }

        //Añadir employee
        //Atributos del metodo
        [HttpPost]
        [Route("GuardarEmpleado")]
        //metodo añadir empleados
        public IActionResult GuardarEmpleado([FromBody] EmployeeEntity employee)
        {
            try
            {
                //Conexion a la base de datos
                using (var conexion = new SqlConnection(cadenaSQL))
                {
                    //Abrir conexion
                    conexion.Open();
                    //Ejecutar procedimiento almacenado sp_add_employee
                    var cmd = new SqlCommand("sp_add_employee", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("NameEmployee", employee.NameEmployee);
                    cmd.Parameters.AddWithValue("Position", employee.Position);
                    cmd.Parameters.AddWithValue("Office", employee.Office);
                    cmd.Parameters.AddWithValue("Salary", employee.Salary);
                    cmd.ExecuteNonQuery();
                }
                //retorno de respuesta exitosa
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Añadido", Response = employee });
            }
            catch (Exception error)
            {
                //si hay un error
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = employee });
            }
        }

        //Editar employee
        //Atributos del metodo
        [HttpPut]
        [Route("EditarEmpleado")]
        //Metodo
        public IActionResult EditarEmpleado([FromBody] EmployeeEntity employee)
        {
            try
            {
                //Conexion a base de datos
                using(var conexion=new SqlConnection(cadenaSQL))
                {
                    //Abrir conexion
                    conexion.Open();
                    //ejecutar procedimiento almacenado sp_update_employee
                    var cmd = new SqlCommand("sp_update_employee", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("IdEmployee", employee.IdEmployee == 0 ? DBNull.Value : employee.IdEmployee);
                    cmd.Parameters.AddWithValue("NameEmployee", employee.NameEmployee is null ? DBNull.Value : employee.NameEmployee);
                    cmd.Parameters.AddWithValue("Position", employee.Position is null ? DBNull.Value : employee.Position);
                    cmd.Parameters.AddWithValue("Office", employee.Office is null ? DBNull.Value : employee.Office);
                    cmd.Parameters.AddWithValue("Salary", employee.Salary == 0 ? DBNull.Value : employee.Salary);
                    cmd.ExecuteNonQuery();
                }
                //retorno de respueta exitosa
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Editado", Response = employee });
            }
            catch(Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, Response = employee });
            }
        }

        //Eliminar employee
        //Atributos del metodo
        [HttpDelete]
        [Route("EliminarEmpleado/{IdEmployee:int}")]
        //Metodo
        public IActionResult EliminarEmpleado(int IdEmployee)
        {
            try
            {
                //Conexion a base de datos
                using(var conexion=new SqlConnection(cadenaSQL))
                {
                    //Abrir conexion 
                    conexion.Open();
                    //Ejecutar procedimiento almacenado sp_delete_employee
                    var cmd = new SqlCommand("sp_delete_employee", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("IdEmployee", IdEmployee);
                    cmd.ExecuteNonQuery();
                }
                //retorno de respuesta exitosa
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Eliminado" });
            }
            catch(Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });   
            }
        }
    }
}
