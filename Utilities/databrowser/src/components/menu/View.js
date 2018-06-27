import React from 'react';
import Typography from '@material-ui/core/Typography';
import AppBar from '@material-ui/core/AppBar';
import Button from '@material-ui/core/Button';
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
import Divider from '@material-ui/core/Divider';
import AvTimerIcon from '@material-ui/icons/AvTimer';
import CodeIcon from '@material-ui/icons/Code';

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
    const { value } = this.state;

    return  <div className={classes.root}>
      <AppBar position="static">
        <Toolbar>
          <IconButton onClick={this.toggleDrawer(true)} className={classes.menuButton} color="inherit" aria-label="Menu">
            <MenuIcon />
          </IconButton>
          <img src={Logo} height='50' />
        </Toolbar>
      </AppBar>
      <SwipeableDrawer
        open={this.state.open}
        onClose={this.toggleDrawer(false)}
        onOpen={this.toggleDrawer(true)}
        >
        <List component="nav">
                <ListItem button onClick={() => this.changeView('timeline')}>
                  <ListItemIcon>
                    <AvTimerIcon />
                  </ListItemIcon>
                  <ListItemText primary="Data Stream" />
                </ListItem>
                <ListItem button onClick={() => this.changeView('code')}>
                  <ListItemIcon>
                    <CodeIcon />
                  </ListItemIcon>
                  <ListItemText primary="Code Editor" />
                </ListItem>
              </List>
      </SwipeableDrawer>
    </div>
  }
}

export default withStyles(styles)(MenuView);
