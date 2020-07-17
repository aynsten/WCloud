import axios from 'axios'
import Qs from 'qs'

//跨域携带cookie
axios.defaults.withCredentials = true;
var axios_param = {
  //baseURL: api_server,
  timeout: 20000 // 请求超时时间
}
//创建 axios 实例
var api_server = process.env.VUE_APP_API_BASE_URL || '';

var api_server_valid =
  api_server.toLowerCase().startsWith('http://') ||
  api_server.toLowerCase().startsWith("https://");

if (api_server_valid) {
  axios_param.baseURL = api_server;
}
const send_ajax = axios.create(axios_param);

//过滤返回数据
function wrap_request(request_promise, vue) {

  var promise_callback = function (resolve, reject) {

    var handle_response = function (response) {
      //success
      var res = response.data;
      var http_code = response.status;
      //console.log(response);

      if (!res.Success) {
        if (res.ErrorCode === '-401' || http_code === 401) {
          //redirect to login
          vue.$store.commit('SET_INFO', {});
          vue.$router.push({ name: 'login' });
        }
      }
      resolve(res);
    };

    var handle_error = function (err) {
      console.log(err);
      reject(err);
    };

    var handle_finally = function () { };

    request_promise.then(handle_response).catch(handle_error).finally(handle_finally);

  };

  return new Promise(promise_callback);
}

//用来添加固定参数
function wrap_param(vue, param) {
  var token = vue.$ls.get('access_token', null);
  if (token) {
    param.headers['Authorization'] = `Bearer ${token}`;
  }
  return param;
}

var get_data = function (vue, url, headers) {
  var param = {
    url: url,
    method: 'get',
    headers: {
      ...(headers || {})
    }
  };
  param = wrap_param(vue, param);
  var request_promise = send_ajax(param);
  return wrap_request(request_promise, vue);
};

var post_form = function (vue, url, data, headers) {
  var param = {
    url: url,
    method: 'post',
    data: Qs.stringify(data),
    headers: {
      ...(headers || {})
    }
  };
  param = wrap_param(vue, param);
  var request_promise = send_ajax(param);
  return wrap_request(request_promise, vue);
};

var post_form_data = function (vue, url, form_data, headers) {
  var param = {
    url: url,
    method: 'post',
    data: form_data,
    headers: {
      ...(headers || {})
    }
  };
  param = wrap_param(vue, param);
  var request_promise = send_ajax(param);
  return wrap_request(request_promise, vue);
};

var post_body = function (vue, url, data, headers) {
  var param = {
    url: url,
    method: 'post',
    data: data,
    headers: {
      ...(headers || {})
    }
  };
  param = wrap_param(vue, param);
  var request_promise = send_ajax(param);
  return wrap_request(request_promise, vue);
};

export { get_data, post_form, post_form_data, post_body }
