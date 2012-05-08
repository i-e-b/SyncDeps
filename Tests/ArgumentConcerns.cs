#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System;
using Machine.Specifications;
using SyncDeps;

// Feature: Tests
// Scenario: ArgumentConcerns
//
// Given: An argument parser
// When: reading command line with base path and source pattern and dest pattern and log path
// Then it: provide all fields



namespace ArgumentConcerns
{
	class When_reading_command_line_with_base_path_and_source_pattern_and_dest_pattern_and_log_path : with.An_argument_parser
	{
		Because of = () => subject.Read(full_arguments);

		It should_provide_the_passed_base_path =()=> subject.BasePath.ShouldEqual(base_path);
		It should_provide_the_passed_source_pattern =()=> subject.SourcePattern.ShouldEqual(source_pattern);
		It should_provide_the_passed_destination_patter =()=> subject.DestPattern.ShouldEqual(dest_pattern);
		It should_provide_the_passed_log_path =()=> subject.LogPath.ShouldEqual(log_path);
	}

	class When_reading_command_line_with_base_path_and_source_pattern_and_dest_pattern : with.An_argument_parser
	{
		Because of =()=> subject.Read(all_but_log_path);

		It should_provide_the_passed_base_path =()=> subject.BasePath.ShouldEqual(base_path);
		It should_provide_the_passed_source_pattern =()=> subject.SourcePattern.ShouldEqual(source_pattern);
		It should_provide_the_passed_destination_patter =()=> subject.DestPattern.ShouldEqual(dest_pattern);
		It should_give_a_null_log_path =()=> subject.LogPath.ShouldBeNull();
	}
	
	class When_reading_command_line_with_base_path_and_source_pattern_and_dest_pattern_and_log_path_having_self_reference_argument : with.An_argument_parser
	{
		Because of = () => subject.Read(full_arguments_with_self_reference);

		It should_provide_the_passed_base_path =()=> subject.BasePath.ShouldEqual(base_path);
		It should_provide_the_passed_source_pattern =()=> subject.SourcePattern.ShouldEqual(source_pattern);
		It should_provide_the_passed_destination_patter =()=> subject.DestPattern.ShouldEqual(dest_pattern);
		It should_provide_the_passed_log_path =()=> subject.LogPath.ShouldEqual(log_path);
	}

	class When_reading_command_line_with_base_path_and_source_pattern_and_dest_pattern_having_self_reference_argument : with.An_argument_parser
	{
		Because of =()=> subject.Read(all_but_log_path_with_self_reference);

		It should_provide_the_passed_base_path =()=> subject.BasePath.ShouldEqual(base_path);
		It should_provide_the_passed_source_pattern =()=> subject.SourcePattern.ShouldEqual(source_pattern);
		It should_provide_the_passed_destination_patter =()=> subject.DestPattern.ShouldEqual(dest_pattern);
		It should_give_a_null_log_path =()=> subject.LogPath.ShouldBeNull();
	}

	
	class When_reading_command_line_with_incomplete_arguments : with.An_argument_parser
	{
		Because it = ShouldFail(() => subject.Read(incomplete_arguments));

		It throw_an_argument_exception =()=> the_exception.ShouldBeOfType<ArgumentException>();
	}

	#region contexts
	namespace with
	{
		[Subject("with An argument parser")]
		public abstract class An_argument_parser : ContextOf<ArgumentParser>
		{
			protected static string base_path = "base path";
			protected static string source_pattern = "src patt";
			protected static string dest_pattern = "dst patt";
			protected static string log_path = "path";
			protected static string log_argument = "-log:\"path\"";
			protected static string self_ref = "random unhelpful string";
			
			protected static string[] incomplete_arguments = new[] { source_pattern, dest_pattern };
			protected static string[] full_arguments = new[] { base_path, source_pattern, dest_pattern, log_argument };
			protected static string[] all_but_log_path = new[] { base_path, source_pattern, dest_pattern };
			protected static string[] full_arguments_with_self_reference = new[] { self_ref, base_path, source_pattern, dest_pattern, log_argument };
			protected static string[] all_but_log_path_with_self_reference = new[] { self_ref, base_path, source_pattern, dest_pattern };

			Establish context = () =>
			{
				subject = new ArgumentParser();
			};
		}
	}
	#endregion
}