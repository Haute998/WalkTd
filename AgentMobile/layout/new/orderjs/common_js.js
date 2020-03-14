$(document).ready(function(){
  
  $('li.hd_menu_tit').mousemove(function(){
  $(this).find('div.hd_menu_list,div.hd_Shopping_list').show();//you can give it a speed
  });
  $('li.hd_menu_tit').mouseleave(function(){
  $(this).find('div.hd_menu_list,div.hd_Shopping_list').hide();
  });
 $(function(){
	$(".fixed_qr_close").click(function(){
		$(".mod_qr").hide();
	})
})
//按钮点击事件
$(".modify_btn").click(function(){
	 $('.text').attr("disabled", false);
	 $('.text').addClass("add");
	  $('#Personal').find('.info_list').addClass("hover");
	
	});
	$(".confirm").click(function(){
		$('.text').attr("disabled", true);
		$('.text').removeClass("add");
		})
});
/*$(document).ready(function(){
	$("#index_search_main .search-container").click(function(){
			$(this).addClass("hd_menu_hover");
			$(this).children("div.search-container").attr('class','');
		},function(){
			$(this).removeClass("hd_menu_hover");  
			$(this).children("div.search-container").attr('class','');
		}
	); 
	$("#nav li.no_sub").hover(function(){
			$(this).addClass("hover1");
		},function(){
			$(this).removeClass("hover1");  
		}
	); 
});*/

$(document).ready(function(){
$("#allSortOuterbox li").hover(function(){
		$(this).find(".menv_Detail").show();
	},function(){
		$(this).find(".menv_Detail").hide();
});
	$("#allSortOuterbox li.name").hover(function(){
												
			$(this).addClass("hover_nav");
												
		},function(){
			$(this).removeClass("hover_nav" );  
		});
		$("div.display ").hover(function(){
		$(this).addClass("hover");
	},function(){
		$(this).removeClass("hover" );  
});	
});
$(document).ready(function(){
$("#lists li").hover(function(){
		$(this).find(".Detailed").show();
	},function(){
		$(this).find(".Detailed").hide();
});
	$("#lists li").hover(function(){
												
			$(this).addClass("hover_nav");
												
		},function(){
			$(this).removeClass("hover_nav" );  
		}
	); 
});
/**********鼠标移动效果************/
$(document).ready(function(){
	$("ul.products").hover(function() {
		$(this).find("#cell_12428").stop()
		.animate({right: "0", opacity:1}, "fast")
		.css("display","block")

	}, function() {
		$(this).find("#cell_12428").stop()
		.animate({right: "0", opacity: 0}, "fast")
	});
});
$(window).scroll(function() {
		var topToolbar = $("#dd_Section");
		var headerH = $("#Group_outerHeight").outerHeight();
		var headers = $("#header_outerHeight").outerHeight();
		var scrollTop =$(document.body).scrollTop();	
			if( scrollTop >= headerH + headers ){
				topToolbar.stop(false,true).addClass("fixToTop");
			}else if( scrollTop < headerH + headers ){
				topToolbar.stop(false,true).removeClass("fixToTop"); 
			}
});
/*********************点击事件*********************/
$( document).ready(function(){
  $('#index_searchForm').die().live("click",function(){ 
	$('#index_search_main').find('.search-container').removeClass('search-container, on-focus').attr('class','').addClass('search-container  on-focus');
	$('#scwrapper')(function(){
												
			$(this).addClass("hover_nav");
												
		},function(){
			$(this).removeClass("hover_nav" );  
		}); 
	  	
  }); 
      $('.sprite-icon').die().live("click",function(){ 
	$('#index_search_main').find('.search-container').attr('class','').addClass('search-container  on-blur');
	//$(this).addClass('search-container  on-focus');
	  	
  });
 });
/***********************商品分类顶部浮动固定层菜单栏**************************/
$(window).scroll(function() {
		var topToolbar = $("#index_search_main");
		var headers = $("#swiper_wrapper").outerHeight();
		var scrollTop =$(document.body).scrollTop();	
			if( scrollTop >= + headers ){
				topToolbar.stop(false,true).addClass("fixToTop");
			}else if( scrollTop < headers ){
				topToolbar.stop(false,true).removeClass("fixToTop"); 
			}
});