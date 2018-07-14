import React from 'react';
import IconButton from '@material-ui/core/IconButton';
import deepOrange from '@material-ui/core/colors/deepOrange';
import deepPurple from '@material-ui/core/colors/deepPurple';
import Avatar from '@material-ui/core/Avatar';
import { withStyles } from '@material-ui/core/styles';
import Chronicity from '../../helpers/Chronicity.js'
import StateView from './StateView.js'
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import MoreVertIcon from '@material-ui/icons/MoreVert';
import CardHeader from '@material-ui/core/CardHeader';

const styles = {
  avatar: {
    margin: 10,
  },
  orangeAvatar: {
    margin: 10,
    color: '#fff',
    backgroundColor: deepOrange[500],
  },
  purpleAvatar: {
    margin: 10,
    color: '#fff',
    backgroundColor: deepPurple[500],
  },
  row: {
    display: 'flex',
    justifyContent: 'center',
  },
  itemStyle : {
    backgroundColor: 'white',
    margin: '20px',
    boxShadow : '0 1px 4px 0 rgba(0, 0, 0, 0.14)',
    borderRadius : '6px'
  },

  card: {
   minWidth: 275,
   margin: '20px',
   borderRadius: '5px'
   },
   bullet: {
     display: 'inline-block',
     margin: '0 2px',
     transform: 'scale(0.8)',
   },
   title: {
     marginBottom: 16,
     fontSize: 14,
   },
   pos: {
     marginBottom: 12,
   },
};

class Event extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      items: null
    };
  }

  openStateViewer = (data) => {

      if(this.state.items != null)
      {
        this.setState({
          items: null
        });
        return;
      }

      var that = this;
      data.entities.forEach(function(e) {
        Chronicity.getEntityState(e,data.on)
        .then(function(data){
            that.addStateViewerItems(e,data);
        });
      });
  };

  addStateViewerItems(entity,items)
  {
    var data = [];

    Object.keys(items).forEach(function(key) {
      data.push({
        entity: entity,
        key: key,
        value: items[key]
      });
    });

    this.setState({
      items: data
    });

  }

  render() {

    const { classes } = this.props;

    return (<Card className={classes.card}  key={this.props.event.id}>
         <CardHeader
            avatar={
              <Avatar style={this.props.event.iconStyle}>{this.props.event.initials}</Avatar>
            }
            action={
              <IconButton onClick={() => this.openStateViewer(this.props.event)}>
                <MoreVertIcon />
              </IconButton>
            }
            title={this.props.event.type}
            subheader={this.props.event.on}
          />

          {this.state.items != null &&
            <CardContent>
            <StateView items={this.state.items} />
            </CardContent>
          }
      </Card>);

  }
}

export default  withStyles(styles)(Event);
