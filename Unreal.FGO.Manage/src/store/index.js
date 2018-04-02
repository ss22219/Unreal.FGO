import Vue from 'vue'
import Vuex from 'vuex'
import { getMapData, getUserInfo, setRole, setTask,
  disableTask, delRole, delTask, getQuests, showNetError, 
  getTaskList, getTaskLogs, getQuestList,getItems,
  battleSetup, battleResult,resetTask,getSvts,getRoleSvts } from './api'
import Promise from 'bluebird'
Vue.use(Vuex)

const store = new Vuex.Store({
  state: {
    battles: {},
    user: {},
    roles: [],
    tasks: [],
    mapData: null,
    svts : [],
    quests: [],
    loading: true,
    error: {
      show: false,
      msg: ''
    }
  },
  actions: {
    SET_ERROR: ({ commit, state },msg) => {
      commit('SET_ERROR',msg)
    },
    CLEAR_LOADING: ({ commit, state }) => {
      commit('CLEAR_LOADING')
    },
    LOAD_MAP_DATA: ({commit, state}) => {
      if (state.mapData)
        return Promise.resolve(state.mapData)
      else
        return getMapData().then(res => {
          commit('SET_MAP_DATA', res.data)
        })
    },
    DELETE_ROLE: ({ commit, state }, role) => {
      return showNetError(delRole(role.id), commit, (res) => {
        store.dispatch('FETCH_ROLE')
      })
    }, SET_ROLE: ({ commit, state}, role) => {
      return showNetError(setRole(role), commit, (res) => {
        store.dispatch('FETCH_ROLE')
      })
    }, DELETE_TASK: ({ commit, state }, task) => {
      return showNetError(delTask(task.id), commit, (res) => {
        store.dispatch('FETCH_TASKS')
      })
    }, DISABLE_TASK: ({ commit, state}, task) => {
      return showNetError(disableTask(task.id), commit, (res) => {
        store.dispatch('FETCH_TASKS')
      })
    },RESET_TASK: ({ commit, state }, task) => {
      return showNetError(resetTask(task.id), commit, (res) => {
        store.dispatch('FETCH_TASKS')
      })
    },
     SET_TASK: ({ commit, state}, task) => {
      return showNetError(setTask(task), commit, (res) => {
        store.dispatch('FETCH_ROLE')
      })
    },
    FETCH_TASKS: ({commit, state}) => {
      return showNetError(getTaskList(), commit, reply => {
        commit('SET_TASKS', reply.data.data)
      });
    },
    FETCH_TASK_LOGS: ({commit, state}, id) => {
      return showNetError(getTaskLogs(id), commit);
    },
    FETCH_ROLE: ({ commit, state }, {  }) => {
      showNetError(getTaskList(), commit, reply => {
        commit('SET_TASKS', reply.data.data)
      });
      return showNetError(getUserInfo(), commit, reply => {
        commit('CLEAR_LOADING')
        commit('SET_DATA', reply.data.data)
      })
    },
    FETCH_ITEM: ({commit, state}, id) => {
      return showNetError(getItems(id), commit, reply => {
        commit('CLEAR_LOADING')
      });
    },
    FETCH_ROLE_SVT: ({commit, state}, id) => {
      return showNetError(getRoleSvts(id), commit, reply => {
        commit('CLEAR_LOADING')
      });
    },
    FETCH_SVT: ({ commit, state }, {  }) => {
      if (state.svts.length)
        return
      return showNetError(getSvts(), commit, reply => {
        commit('CLEAR_LOADING')
        commit('SET_SVT', reply.data.data)
      })
    },
    FETCH_QUEST: ({ commit, state }, {  }) => {
      if (state.quests.length)
        return
      return showNetError(getQuests(), commit, reply => {
        commit('CLEAR_LOADING')
        commit('SET_QUEST', reply.data.data)
      })
    },
    FETCH_ROLE_QUEST: ({ commit, state }, roleId) => {
      return showNetError(getQuestList(roleId), commit, reply => {
        return reply
      })
    },
    BATTLE_SETUP: ({ commit, state }, param) => {
      return showNetError(battleSetup(param), commit, reply => {
        commit('SET_BATTLE', {roleId : param.roleId,battleId : reply.data.data.battleId })
      })
    },
    BATTLE_RESULT: ({ commit, state }, param) => {
      return showNetError(battleResult(param), commit, reply => {
          if(reply.data.code == 0)
            commit('CLEAR_BATTLE', param)
      })
    }
  },
  mutations: {
    SET_BATTLE:(state, {roleId, battleId})=>{
        state.battles[roleId] = {battleId, time : new Date()}
    },
    CLEAR_BATTLE:(state, {roleId})=>{
        state.battles[roleId] = false
    },
    SET_MAP_DATA: (state, data) => {
      state.mapData = data
    },
    SET_TASKS: (state, {tasks, lastLogs}) => {
      tasks.forEach(t => {
        if (lastLogs[t.id])
          t.lastLog = lastLogs[t.id]
      })
      state.tasks = tasks
    },
    SET_LOADING: (state) => {
      state.loading = true
    },
    CLEAR_LOADING: (state) => {
      state.loading = false
    },
    SET_ERROR: (state, msg) => {
      state.error.show = true
      state.error.msg = msg
    },
    CLEAR_ERROR: (state) => {
      state.error.show = false
    },
    SET_QUEST: (state, quests) => {
      state.quests = quests
    },
    SET_SVT: (state, svts) => {
      svts.forEach(s=>{
        s.name = s.name + ' id:' + s.id 
      })
      state.svts = svts
    },
    SET_DATA: (state, { roles, roleDatas, devices, tasks, battles }) => {
      devices.forEach(device => {
        if (!device)
          return;
        var role = roles.find(role => role.device_id == device.id);
        if (!role)
          return;
        var rid = role.id
        role = Object.assign(role, device)
        role.id = rid
      })
      if(battles && battles.length)
      battles.forEach(b=> {
        if(b && b.battleId)
          state.battles[b.roleId] = {battleId : b.battleId}
      })
      state.roles = roles
      state.tasks = tasks
    }
  },

  getters: {
    // ids of the items that should be currently displayed based on
    // current list type and current pagination
    roles(state) {
      return state.roles
    },
    // items that should be currently displayed.
    // this Array may not be fully fetched.
    tasks(state) {
      return state.tasks
    },
    // items that should be currently displayed.
    // this Array may not be fully fetched.
    devices(state) {
      return state.devices
    }
  }
})
export default store
