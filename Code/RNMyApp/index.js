/**
 * @format
 */
import "./url/storage"
import Http from "./url/feach";
let obj={password:'123qwe',usernameOrEmailAddress:'admin'}
        Http.post('/api/TokenAuth/AdminAuthenticate', obj).then(res => {
            storage.save({
                key: 'loginState',
                data: {
                  token: res.result.accessToken
                }
              })
        }) 
import {AppRegistry} from 'react-native';
import App from './App';
import Index from './components/Index';
import {name as appName} from './app.json';


AppRegistry.registerComponent(appName, () => App);
