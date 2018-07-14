import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Logo from '../../images/logo.png'
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import { withStyles } from '@material-ui/core/styles';
import SwipeableDrawer from '@material-ui/core/SwipeableDrawer';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import AvTimerIcon from '@material-ui/icons/AvTimer';

const styles = {
  root: {
    flexGrow: 1,
  },
  flex: {
    flex: 1,
  },
  menuButton: {
    marginLeft: -12,
    marginRight: 20,
  },
};

class MenuView extends React.Component {

  state = {
    open: false
  };

  handleSelect = (event) => {
    alert(event);
  };

  toggleDrawer = (value) => () => {
    this.setState({
       open: value
    });
  };

  changeView = (view) => {
    this.props.changeView(view);
    this.setState({
       open: false
    });
  }

  render() {
    const { classes } = this.props;

    return  <div className={classes.root}>
      <AppBar position="static">
        <Toolbar>
          <IconButton onClick={this.toggleDrawer(true)} color="inherit" aria-label="Menu">
            <MenuIcon />
          </IconButton>
          <img src={Logo} height='50' alt='' />
        </Toolbar>
      </AppBar>
      <SwipeableDrawer
        open={this.state.open}
        onClose={this.toggleDrawer(false)}
        onOpen={this.toggleDrawer(true)}
        >
        <List component="nav">
          <ListItem button onClick={() => this.changeView('birdactivity')}>
            <ListItemIcon>
              <AvTimerIcon />
            </ListItemIcon>
            <ListItemText primary="Bird Activity" />
          </ListItem>
        </List>
      </SwipeableDrawer>
    </div>
  }
}

export default withStyles(styles)(MenuView);
