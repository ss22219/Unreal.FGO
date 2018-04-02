import Vue from 'vue'
const API_ROOT = 'http://localhost:22080/'
export const getUserInfo = () => Vue.http.get(API_ROOT + '/manage/userInfo');
export const getQuests = () => Vue.http.get(API_ROOT + '/manage/quests');
export const getSvts = () => Vue.http.get(API_ROOT + '/manage/svts');
export const setRole = (options) => Vue.http.post(API_ROOT + '/manage/role', options);
export const getTaskList = () => Vue.http.get(API_ROOT + '/manage/taskList');
export const getTaskLogs = (taskId) => Vue.http.get(API_ROOT + '/manage/taskLog?id=' + taskId);
export const setTask = (options) => Vue.http.post(API_ROOT + '/manage/task', options);
export const disableTask = (id) => Vue.http.post(API_ROOT + '/manage/disableTask?id=' + id);
export const resetTask = (id) => Vue.http.post(API_ROOT + '/manage/resetTask?id=' + id);
export const delRole = (id) => Vue.http.get(API_ROOT + '/manage/delRole?id=' + id);
export const delTask = (id) => Vue.http.get(API_ROOT + '/manage/delTask?id=' + id);
export const getMapData = () => Vue.http.get(API_ROOT + '/user/getMapData');
export const getQuestList = (roleId) => Vue.http.get(API_ROOT + '/manage/questList?roleId=' + roleId);
export const getItems = (roleId) => Vue.http.get(API_ROOT + '/manage/items?roleId=' + roleId);
export const getRoleSvts = (roleId) => Vue.http.get(API_ROOT + '/manage/roleSvts?roleId=' + roleId);

export const battleSetup = (options) => Vue.http.post(API_ROOT + '/manage/battleSetup', options);
export const battleResult = (options) => Vue.http.post(API_ROOT + '/manage/battleResult', options);

export const showNetError = (req, commit, call) => {
  commit('SET_LOADING')
  req.then((res) => {
    commit('CLEAR_LOADING')
    if (res.data != null && res.data.code != 0) {
      if (res.data.code == -500) {
        commit('SET_ERROR', "服务器内部错误")
        console.log(res.data.message)
      }
      else
        commit('SET_ERROR', res.data.message)
      setTimeout(() => commit('CLEAR_ERROR'), 2500)
    }
    else
      call && call(res);
  }, () => {
    commit('CLEAR_LOADING')
    commit('SET_ERROR', '网络请求失败')
    setTimeout(() => commit('CLEAR_ERROR'), 2500)
  })
  return req
}