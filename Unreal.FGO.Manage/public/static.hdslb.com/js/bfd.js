$(function(){
  window._czc = window._czc || [];
  $('.top-list .v-item > a').live('click',function(){_czc.push(["_trackEvent",'top_reco','click', $(this).attr('href').match(/av\d+/).toString()]);});
  $('.v-list .v > a').live('click',function(){_czc.push(["_trackEvent",'v','click']);});
  $('.rlist li > a').live('click',function(){_czc.push(["_trackEvent",'rank','click']);});
  $('#i_menu_msg_btn .dyn_list_wrapper a').live('click',function(){_czc.push(["_trackEvent",'_msg','click']);});
  $('#i_menu_fav_btn li a').live('click',function(){_czc.push(["_trackEvent",'_fav','click']);});
  $('#i_menu_history_btn li a').live('click',function(){_czc.push(["_trackEvent",'_history','click']);});
  $('.read-push').bind('click',function(){_czc.push(['_trackEvent','vfresh','click']);});
  $('#b_promote li a').bind('click',function(){_czc.push(["_trackEvent",'promote','click',$(this).attr('href').match(/av\d+/).toString()]);});
  $('#b_recommend .preview').bind('click',function(){_czc.push(["_trackEvent",'user_reco','click', $(this).attr('href').match(/av\d+/).toString()]);});
  $('.pmt-item a').live('click',function(){_czc.push(["_trackEvent",'low_banner','click',$(this).attr('href').replace($(location).attr('protocol') + '//' + $(location).attr('host'),'')]);});
  $('.topic-preview li>a').live('click',function(){_czc.push(['_trackEvent','big_banner','click',$(this).attr('href').replace($(location).attr('protocol') + '//' + $(location).attr('host'),'')]);});
  $('.qr-client-link').live('click',function(){_czc.push(["_trackEvent",'qr_client_link','click', $(this).children('span').text()]);});
  $(document).on('click','[b-stat]',function(){var ele = $(this);if(ele.attr("b-stat-v")){_czc.push(["_trackEvent", ele.attr("b-stat"), 'click', ele.attr("b-stat-v")]);}else{_czc.push(["_trackEvent", ele.attr("b-stat"), 'click']);}});
});