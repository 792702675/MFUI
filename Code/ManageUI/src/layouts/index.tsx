import React from 'react';
import styles from './index.css';
import { LocaleProvider } from 'antd';
import zhCN from 'antd/lib/locale-provider/zh_CN';
const Sb: React.FC = props => {
  return (
    <LocaleProvider  className={styles.normal} locale={zhCN}>
      {props.children}
    </LocaleProvider >
  );
};

export default Sb;
