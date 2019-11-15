
let dev = "http://220.165.143.88:22222";
export default class Http {
  // 静态方法


  static get(url, params) {
    url = dev + url;
    if (params) {
      let paramsArray = [];
      Object.keys(params).forEach(key =>
        paramsArray.push(key + "=" + params[key])
      );
      if (url.search(/\?/) === -1) {
        url = url + "?" + paramsArray.join("&");
      } else {
        url = url + "&" + paramsArray.join("&");
      }
    }
    let token;
    let header = {
      withCredentials: true,
      credentials: 'include',
      'Accept': 'application/json',
    };
    return new Promise((resolve, reject) => {
      storage.load({
        key: 'loginState',
      }).then(ret => {
        token = ret.token;
        if (token) {
          header['Authorization'] = 'Bearer ' + token
        }
        fetch(url, { method: "GET", headers: header })
          .then(response => response.json())
          .then(resulet => {
            resolve(resulet);
          })
          .catch(error => {
            reject(error);
          });
      })
    });
  }
  static post(url, params) {
    url = dev + url;
    let token;
    storage.load({
      key: 'loginState',
    }).then(ret => {
      token = ret.token;
    })
    let header = {
      withCredentials: true,
      credentials: 'include',
      'Accept': 'application/json',
      'Content-Type': 'application/json',
    };
    if (token) {
      header['Authorization'] = 'Bearer ' + token
    }
    return new Promise(function (resolve, reject) {
      fetch(url, {
        method: 'POST',
        headers: header,
        body: JSON.stringify(params)
      }).then((response) => response.json())
        .then(resulet => {
          resolve(resulet);
        })
        .catch((err) => {
          reject(err);
        });
    });
  }
  static upload(url,params){
    console.log(params)
      return new Promise(function (resolve, reject) {
          let formData = new FormData();
          formData.append('file', {
            name: params.path.substring(params.path.lastIndexOf("/")+1),
            // type:'multipart/form-data',
            type: 'multipart/form-data',
            uri:params.uri
            // ...params
          });
          console.log(formData)
          storage.getBatchData([
            { key: 'loginState' },
            { key: 'bucketsname' },
            { key: 'roots' }
        ])
        .then(results => {
          // results.forEach( result => {
            console.log(dev + url+"?bucketName="+results[1].bucketsname+"&path="+results[2].roots)
            fetch(dev + url+"?bucketName="+results[1].bucketsname+"&path="+results[2].roots, {
              method: 'POST',
              headers: {
                "Content-Type":"multipart/form-data",
                authorization: 'Bearer ' + results[0].token
              },
              body: formData,
          }).then((response) => response.json())
              .then((responseData)=> {
                  console.log('uploadImage', responseData); 
                  resolve(responseData);
              })
              .catch((err)=> {
                  console.log('err', err);
                  reject(err);
              });
          })
        // })
          
      });
  }
}