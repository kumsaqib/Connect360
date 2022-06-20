using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using Connect360.Models;

namespace Connect360.Controllers
{

    [Route("api/1.0/[controller]")]
    public class SetupController : ApiController
    {
        string serviceBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"];

        [Route("RegisterECR")]
        [HttpPost]
        public IHttpActionResult Ecr_register([FromBody] register_model model)
        {
            string ReturnMessage = string.Empty;

            try
            {
                if (!ModelState.IsValid)
                {
                    ReturnMessage = string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage));
                    return BadRequest(ReturnMessage);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serviceBaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

                    //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
                    //client.DefaultRequestHeaders.Add("Password", "admin");

                    HttpResponseMessage response = client.PostAsJsonAsync("api/register", model).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsAsync<dynamic>().Result;
                        return Ok(result);
                    }
                    return StatusCode(response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }


        [Route("PosSignIn")]
        [HttpPost]
        public IHttpActionResult Sign_in([FromBody] sign_in_model model)
        {
            string ReturnMessage = string.Empty;

            try
            {
                if (!ModelState.IsValid)
                {
                    ReturnMessage = string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage));
                    return BadRequest(ReturnMessage);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serviceBaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

                    //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
                    //client.DefaultRequestHeaders.Add("Password", "admin");
                    registered_users_model _ = new registered_users_model()
                    {
                        host = model.host
                    };
                    var response = client.PostAsJsonAsync("api/registered_users", _).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var _registered_user = response.Content.ReadAsAsync<string[]>().Result;
                        if (!_registered_user.Contains(model.pos.ToLower()))
                        {
                            register_model __ = new register_model()
                            {
                                user = model.pos,
                                password = model.password,
                                host = model.host
                            };
                            response = client.PostAsJsonAsync("api/register", __).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                if (_registered_user.Contains(model.ecr.ToLower()))
                                {
                                    get_roster_model ___ = new get_roster_model()
                                    {
                                        host = model.host,
                                        user = model.ecr
                                    };
                                    response = client.PostAsJsonAsync("api/get_roster", ___).Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var _roster = response.Content.ReadAsAsync<List<get_roster_responsemodel>>().Result.FirstOrDefault();
                                        if (_roster != null)
                                        {
                                            if (model.IsAdmin)
                                            {
                                                delete_rosteritem_model ____1 = new delete_rosteritem_model()
                                                {
                                                    localuser = _roster.jid.Replace("@localhost", ""),
                                                    localhost = model.host,
                                                    user = model.ecr,
                                                    host = model.host,
                                                };
                                                response = client.PostAsJsonAsync("api/delete_rosteritem", ____1).Result;

                                                delete_rosteritem_model ____2 = new delete_rosteritem_model()
                                                {
                                                    localuser = model.ecr,
                                                    localhost = model.host,
                                                    user = _roster.jid.Replace("@localhost", ""),
                                                    host = model.host,
                                                };
                                                response = client.PostAsJsonAsync("api/delete_rosteritem", ____2).Result;

                                                add_rosteritem_model ____11 = new add_rosteritem_model()
                                                {
                                                    localuser = model.pos,
                                                    localhost = model.host,
                                                    user = model.ecr,
                                                    host = model.host,
                                                    nick = "",
                                                    group = "",
                                                    subs = "both"
                                                };
                                                response = client.PostAsJsonAsync("api/add_rosteritem", ____11).Result;

                                                add_rosteritem_model ____22 = new add_rosteritem_model()
                                                {
                                                    localuser = model.ecr,
                                                    localhost = model.host,
                                                    user = model.pos,
                                                    host = model.host,
                                                    nick = "",
                                                    group = "",
                                                    subs = "both"
                                                };
                                                response = client.PostAsJsonAsync("api/add_rosteritem", ____22).Result;

                                                if (response.IsSuccessStatusCode)
                                                {
                                                    var _addedroster = response.Content.ReadAsAsync<string>().Result;
                                                    message_model _message = new message_model()
                                                    {
                                                        type = "normal",
                                                        from = model.pos + "@" + model.host,
                                                        to = model.ecr + "@" + model.host,
                                                        subject = "Pairing",
                                                        body = model.pos
                                                    };
                                                    response = client.PostAsJsonAsync("api/send_message", _message).Result;
                                                    if (response.IsSuccessStatusCode)
                                                        return Ok();
                                                }
                                            }

                                            else
                                            {
                                                if (_roster.jid == model.pos + "@" + model.host)
                                                {
                                                    message_model _message = new message_model()
                                                    {
                                                        type = "normal",
                                                        from = model.pos + "@" + model.host,
                                                        to = model.ecr + "@" + model.host,
                                                        subject = "Pairing",
                                                        body = model.pos
                                                    };
                                                    response = client.PostAsJsonAsync("api/send_message", _message).Result;
                                                    if (response.IsSuccessStatusCode)
                                                        return Ok();
                                                }

                                                return Ok(new { Code = 500, Message = model.ecr + "is already attached" });
                                            }
                                        }
                                        else
                                        {
                                            add_rosteritem_model ____1 = new add_rosteritem_model()
                                            {
                                                localuser = model.pos,
                                                localhost = model.host,
                                                user = model.ecr,
                                                host = model.host,
                                                nick = "",
                                                group = "",
                                                subs = "both"
                                            };
                                            response = client.PostAsJsonAsync("api/add_rosteritem", ____1).Result;

                                            add_rosteritem_model ____2 = new add_rosteritem_model()
                                            {
                                                localuser = model.ecr,
                                                localhost = model.host,
                                                user = model.pos,
                                                host = model.host,
                                                nick = "",
                                                group = "",
                                                subs = "both"
                                            };
                                            response = client.PostAsJsonAsync("api/add_rosteritem", ____2).Result;

                                            if (response.IsSuccessStatusCode)
                                            {
                                                var _addedroster = response.Content.ReadAsAsync<string>().Result;
                                                message_model _message = new message_model()
                                                {
                                                    type = "normal",
                                                    from = model.pos + "@" + model.host,
                                                    to = model.ecr + "@" + model.host,
                                                    subject = "Pairing",
                                                    body = model.pos
                                                };
                                                response = client.PostAsJsonAsync("api/send_message", _message).Result;
                                                if (response.IsSuccessStatusCode)
                                                    return Ok();
                                            }
                                        }
                                    }
                                }
                                else
                                    return Ok(new { Code = 404, Message = "Please first register the ECR-" + model.ecr + " ID" });
                            }
                        }
                        else
                        {
                            if (_registered_user.Contains(model.ecr.ToLower()))
                            {
                                get_roster_model ___ = new get_roster_model()
                                {
                                    host = model.host,
                                    user = model.ecr
                                };
                                response = client.PostAsJsonAsync("api/get_roster", ___).Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    var _roster = response.Content.ReadAsAsync<List<get_roster_responsemodel>>().Result.FirstOrDefault();
                                    if (_roster != null)
                                    {
                                        if (model.IsAdmin)
                                        {
                                            delete_rosteritem_model ____1 = new delete_rosteritem_model()
                                            {
                                                localuser = _roster.jid.Replace("@localhost", ""),
                                                localhost = model.host,
                                                user = model.ecr,
                                                host = model.host,
                                            };
                                            response = client.PostAsJsonAsync("api/delete_rosteritem", ____1).Result;

                                            delete_rosteritem_model ____2 = new delete_rosteritem_model()
                                            {
                                                localuser = model.ecr,
                                                localhost = model.host,
                                                user = _roster.jid.Replace("@localhost", ""),
                                                host = model.host,
                                            };
                                            response = client.PostAsJsonAsync("api/delete_rosteritem", ____2).Result;

                                            add_rosteritem_model ____11 = new add_rosteritem_model()
                                            {
                                                localuser = model.pos,
                                                localhost = model.host,
                                                user = model.ecr,
                                                host = model.host,
                                                nick = "",
                                                group = "",
                                                subs = "both"
                                            };
                                            response = client.PostAsJsonAsync("api/add_rosteritem", ____11).Result;

                                            add_rosteritem_model ____22 = new add_rosteritem_model()
                                            {
                                                localuser = model.ecr,
                                                localhost = model.host,
                                                user = model.pos,
                                                host = model.host,
                                                nick = "",
                                                group = "",
                                                subs = "both"
                                            };
                                            response = client.PostAsJsonAsync("api/add_rosteritem", ____22).Result;

                                            if (response.IsSuccessStatusCode)
                                            {
                                                var _addedroster = response.Content.ReadAsAsync<string>().Result;
                                                message_model _message = new message_model()
                                                {
                                                    type = "normal",
                                                    from = model.pos + "@" + model.host,
                                                    to = model.ecr + "@" + model.host,
                                                    subject = "Pairing",
                                                    body = model.pos
                                                };
                                                response = client.PostAsJsonAsync("api/send_message", _message).Result;
                                                if (response.IsSuccessStatusCode)
                                                    return Ok();
                                            }
                                        }

                                        else
                                        {
                                            if (_roster.jid == model.pos + "@" + model.host)
                                            {
                                                message_model _message = new message_model()
                                                {
                                                    type = "normal",
                                                    from = model.pos + "@" + model.host,
                                                    to = model.ecr + "@" + model.host,
                                                    subject = "Pairing",
                                                    body = model.pos
                                                };
                                                response = client.PostAsJsonAsync("api/send_message", _message).Result;
                                                if (response.IsSuccessStatusCode)
                                                    return Ok();
                                            }

                                            return Ok(new { Code = 500, Message = model.ecr + " is already attached with " + _roster.jid });
                                        }
                                            
                                    }
                                    else
                                    {
                                        add_rosteritem_model ____1 = new add_rosteritem_model()
                                        {
                                            localuser = model.pos,
                                            localhost = model.host,
                                            user = model.ecr,
                                            host = model.host,
                                            nick = "",
                                            group = "",
                                            subs = "both"
                                        };
                                        response = client.PostAsJsonAsync("api/add_rosteritem", ____1).Result;

                                        add_rosteritem_model ____2 = new add_rosteritem_model()
                                        {
                                            localuser = model.ecr,
                                            localhost = model.host,
                                            user = model.pos,
                                            host = model.host,
                                            nick = "",
                                            group = "",
                                            subs = "both"
                                        };
                                        response = client.PostAsJsonAsync("api/add_rosteritem", ____2).Result;

                                        if (response.IsSuccessStatusCode)
                                        {
                                            message_model _message = new message_model()
                                            {
                                                type = "normal",
                                                from = model.pos + "@" + model.host,
                                                to = model.ecr + "@" + model.host,
                                                subject = "Pairing",
                                                body = model.pos
                                            };
                                            response = client.PostAsJsonAsync("api/send_message", _message).Result;
                                            if (response.IsSuccessStatusCode)
                                                return Ok();
                                        }
                                    }
                                }
                            }
                            else
                                return Ok(new { Code = 404, Message = "Please first register the ECR-" + model.ecr + " ID" });
                        }
                    }
                    return StatusCode(response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }


        [Route("PosSignOut")]
        [HttpPost]
        public IHttpActionResult PosSignOut([FromBody] sign_in_model model)
        {
            string ReturnMessage = string.Empty;

            try
            {
                if (!ModelState.IsValid)
                {
                    ReturnMessage = string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage));
                    return BadRequest(ReturnMessage);
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(serviceBaseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "*/*");
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

                    //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
                    //client.DefaultRequestHeaders.Add("Password", "admin");
                    registered_users_model _ = new registered_users_model()
                    {
                        host = model.host
                    };
                    var response = client.PostAsJsonAsync("api/registered_users", _).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var _registered_user = response.Content.ReadAsAsync<string[]>().Result;
                        if (_registered_user.Contains(model.ecr.ToLower()) && _registered_user.Contains(model.pos.ToLower()) )
                        {
                            get_roster_model ___ = new get_roster_model()
                            {
                                host = model.host,
                                user = model.ecr
                            };
                            response = client.PostAsJsonAsync("api/get_roster", ___).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                var _roster = response.Content.ReadAsAsync<List<get_roster_responsemodel>>().Result.FirstOrDefault();
                                if (_roster != null)
                                {
                                    if (_roster.jid == model.pos + "@" + model.host)
                                    {
                                        delete_rosteritem_model ____1 = new delete_rosteritem_model()
                                        {
                                            localuser = model.pos,
                                            localhost = model.host,
                                            user = model.ecr,
                                            host = model.host,
                                        };
                                        response = client.PostAsJsonAsync("api/delete_rosteritem", ____1).Result;

                                        delete_rosteritem_model ____2 = new delete_rosteritem_model()
                                        {
                                            localuser = model.ecr,
                                            localhost = model.host,
                                            user = model.pos,
                                            host = model.host,
                                        };
                                        response = client.PostAsJsonAsync("api/delete_rosteritem", ____2).Result;

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var _addedroster = response.Content.ReadAsAsync<string>().Result;
                                            message_model _message = new message_model()
                                            {
                                                type = "normal",
                                                from = model.pos + "@" + model.host,
                                                to = model.ecr + "@" + model.host,
                                                subject = "UnPairing",
                                                body = model.pos
                                            };
                                            response = client.PostAsJsonAsync("api/send_message", _message).Result;
                                            if (response.IsSuccessStatusCode)
                                                return Ok();
                                        }
                                    }
                                    else
                                        return Ok();
                                }
                                else
                                    return Ok();
                            }
                        }
                        return Ok();
                    }
                    return StatusCode(response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }



        //[Route("PosRegister")]
        //[HttpPost]
        //public IHttpActionResult pos_register([FromBody] register_model model)
        //{

        //    string ReturnMessage = string.Empty;

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            ReturnMessage = string.Join("; ", ModelState.Values
        //                              .SelectMany(x => x.Errors)
        //                              .Select(x => x.ErrorMessage));
        //            return BadRequest(ReturnMessage);
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(serviceBaseUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Add("Accept", "*/*");
        //            client.DefaultRequestHeaders.Accept.Add(
        //                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

        //            //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
        //            //client.DefaultRequestHeaders.Add("Password", "admin");

        //            HttpResponseMessage response = client.PostAsJsonAsync("api/register", model).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var result = response.Content.ReadAsAsync<dynamic>().Result;
        //                return Ok(result);
        //            }
        //            return StatusCode(response.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }

        //}


        //[HttpPost]
        //public IHttpActionResult add_rosteritem([FromBody] add_rosteritem_model model)
        //{

        //    string ReturnMessage = string.Empty;

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            ReturnMessage = string.Join("; ", ModelState.Values
        //                              .SelectMany(x => x.Errors)
        //                              .Select(x => x.ErrorMessage));
        //            return BadRequest(ReturnMessage);
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(serviceBaseUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Add("Accept", "*/*");
        //            client.DefaultRequestHeaders.Accept.Add(
        //                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

        //            //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
        //            //client.DefaultRequestHeaders.Add("Password", "admin");

        //            HttpResponseMessage response = client.PostAsJsonAsync("api/add_rosteritem", model).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var result = response.Content.ReadAsAsync<dynamic>().Result;
        //                return Ok(result);
        //            }
        //            return StatusCode(response.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }

        //}




        //[HttpPost]
        //public IHttpActionResult registered_users([FromBody] registered_users_model model)
        //{

        //    string ReturnMessage = string.Empty;

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            ReturnMessage = string.Join("; ", ModelState.Values
        //                              .SelectMany(x => x.Errors)
        //                              .Select(x => x.ErrorMessage));
        //            return BadRequest(ReturnMessage);
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(serviceBaseUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Add("Accept", "*/*");
        //            client.DefaultRequestHeaders.Accept.Add(
        //                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

        //            //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
        //            //client.DefaultRequestHeaders.Add("Password", "admin");

        //            HttpResponseMessage response = client.PostAsJsonAsync("api/registered_users", model).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var result = response.Content.ReadAsAsync<dynamic>().Result;
        //                return Ok(result);
        //            }
        //            return StatusCode(response.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }

        //}

        //[HttpPost]
        //public IHttpActionResult connected_users()
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(serviceBaseUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Add("Accept", "*/*");
        //            client.DefaultRequestHeaders.Accept.Add(
        //                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

        //            //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
        //            //client.DefaultRequestHeaders.Add("Password", "admin");

        //            HttpResponseMessage response = client.PostAsJsonAsync("api/connected_users", new { }).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var result = response.Content.ReadAsAsync<dynamic>().Result;
        //                return Ok(result);
        //            }
        //            return StatusCode(response.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }

        //}

        //[HttpPost]
        //public IHttpActionResult get_roster([FromBody] get_roster_model model)
        //{
        //    string ReturnMessage = string.Empty;

        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            ReturnMessage = string.Join("; ", ModelState.Values
        //                              .SelectMany(x => x.Errors)
        //                              .Select(x => x.ErrorMessage));
        //            return BadRequest(ReturnMessage);
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(serviceBaseUrl);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Add("Accept", "*/*");
        //            client.DefaultRequestHeaders.Accept.Add(
        //                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW5AbG9jYWxob3N0OmFkbWlu");

        //            //client.DefaultRequestHeaders.Add("Username", "admin@localhost");
        //            //client.DefaultRequestHeaders.Add("Password", "admin");

        //            HttpResponseMessage response = client.PostAsJsonAsync("api/get_roster", model).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var result = response.Content.ReadAsAsync<dynamic>().Result;
        //                return Ok(result);
        //            }
        //            return StatusCode(response.StatusCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }

        //}

    }
}
