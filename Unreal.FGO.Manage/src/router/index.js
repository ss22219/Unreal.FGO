import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

import HomeView from '../views/HomeView.vue'
import RolesView from '../views/RolesView.vue'
import RoleView from '../views/RoleView.vue'
import TasksView from '../views/TasksView.vue'
import TaskView from '../views/TaskView.vue'
import MapsView from '../views/MapsView.vue'
import ItemsView from '../views/ItemsView.vue'
import BattleView from '../views/BattleView.vue'
import TaskLogsView from '../views/TaskLogsView.vue'
import SvtsView from '../views/SvtsView.vue'

export default new Router({
  mode: 'hash',
  scrollBehavior: () => ({ y: 0 }),
  routes: [
    { path: '/role', component: RoleView},
    { path: '/role/:id', component: RoleView},
    { path: '/roles', component: RolesView  },
    { path: '/tasks', component: TasksView  },
    { path: '/task', component: TaskView  },
    { path: '/task/:id', component: TaskView},
    { path: '/task/:id/logs', component: TaskLogsView},
    { path: '/top', component: HomeView  },
    { path: '/maps', component: MapsView  },
    { path: '/maps/:id', component: MapsView  },
    { path: '/items/:id', component: ItemsView  },
    { path: '/role/:id/svts', component: SvtsView  },
    { path: '/battle/:roleId/:questId', component: BattleView  },
    { path: '/', redirect: '/top'  }
  ]
})
