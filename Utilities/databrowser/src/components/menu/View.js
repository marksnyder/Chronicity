import React from 'react';
import Typography from '@material-ui/core/Typography';
import AppBar from '@material-ui/core/AppBar';
import Button from '@material-ui/core/Button';
import Toolbar from '@material-ui/core/Toolbar';
import Logo from '../../images/logo.png'

class MenuView extends React.Component {

  state = {
    value: 0,
  };

  handleSelect = (event) => {
    alert(event);
  };

  render() {
    const { classes } = this.props;
    const { value } = this.state;

    return  <div className={classes.root}>
      <AppBar position="static">
        <Toolbar>
          <img src={Logo} height='50' />
          <Button color="inherit"  onClick={() => { this.props.changeView('timeline'); }} >Timeline</Button>
        </Toolbar>

      </AppBar>
    </div>
  }
}

export default (MenuView);
